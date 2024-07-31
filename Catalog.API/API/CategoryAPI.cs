using Catalog.API.Applications.Commands.Categories.CreateCategory;
using Catalog.API.Applications.Commands.Categories.DeleteCategory;
using Catalog.API.Applications.Commands.Categories.UpdateCategory;
using Catalog.API.Applications.Queries.Categories.GetCategoryById;
using Catalog.API.Applications.Queries.Categories.GetCategoryPaginated;
using Catalog.API.DTOs.Category;
using Catalog.API.SeedWork;
using Catalog.API.Services;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;

namespace Catalog.API.API;

public static class CategoryAPI
{
    public static IEndpointRouteBuilder CategoryRouter(this IEndpointRouteBuilder route, string groupName)
    {
        var app = route.MapGroup(groupName);

        app.MapGet("/categories/{categoryId}", GetCategoryById);
        app.MapGet("/categories", GetPaginatedCategory);
        app.MapPost("/categories", CreateNewCategory);
        app.MapPut("/categories/{categoryId}", UpdateCategoryById);
        app.MapDelete("/categories/{categoryId}", DeleteCategoryById);

        return app;
    }

    private static async Task<IResult> GetCategoryById(
        string categoryId,
        ApplicationService service,
        IValidator<GetCategoryByIdQuery> validator)
    {
        try
        {
            var query = new GetCategoryByIdQuery(categoryId);

            var validation = await validator.ValidateAsync(query);

            if (!validation.IsValid)
            {
                var errors = validation.Errors.Select(x => x.ErrorMessage).ToList();
                var validationErrors = Result<CategoryDto>.Failure(errors, 400);
                return TypedResults.BadRequest(validationErrors);
            }

            var response = await service.Mediator.Send(query);

            if (response.Success)
            {
                return TypedResults.Ok(response);
            }

            return response.StatusCode switch
            {
                404 => TypedResults.NotFound(response),
                _ => TypedResults.BadRequest(response),
            };
        }
        catch (Exception ex)
        {
            service.Logger.LogCritical("Critical failure: {ErrorMessage}", ex.Message);
            return TypedResults.InternalServerError();
        }
    }

    private static async Task<IResult> GetPaginatedCategory([AsParameters] GetCategoryPaginatedQuery itemPaginatedQuery, ApplicationService service, IValidator<GetCategoryPaginatedQuery> validator)
    {
        try
        {
            var validation = await validator.ValidateAsync(itemPaginatedQuery);

            if (!validation.IsValid)
            {
                var errors = validation.Errors.Select(x => x.ErrorMessage).ToList();
                var validationErrors = Result<CategoryDto>.Failure(errors, 400);
                return TypedResults.BadRequest(validationErrors);
            }

            var response = await service.Mediator.Send(itemPaginatedQuery);

            if (response.Success)
            {
                return TypedResults.Ok(response);
            }

            return response.StatusCode switch
            {
                404 => TypedResults.NotFound(response),
                _ => TypedResults.BadRequest(response),
            };
        }
        catch (Exception ex)
        {
            service.Logger.LogCritical("Critical failure: {ErrorMessage}", ex.Message);
            return TypedResults.InternalServerError();
        }
    }

    private static async Task<IResult> CreateNewCategory(
       [FromBody] CreateCategoryCommand command,
       ApplicationService service,
       IValidator<CreateCategoryCommand> validator)
    {
        try
        {
            var validation = await validator.ValidateAsync(command);
            if (!validation.IsValid)
            {
                var errors = validation.Errors.Select(x => x.ErrorMessage).ToList();
                var validationErrors = Result<CreateCategoryCommand>.Failure(errors, 400);
                return TypedResults.BadRequest(validationErrors);
            }

            var response = await service.Mediator.Send(command);

            if (response.Success)
            {
                response.WithLink(service.LinkGenerator.GetPathByName(nameof(GetCategoryById)) ?? "");

                return TypedResults.Created(service.LinkGenerator.GetPathByName(nameof(GetCategoryById)), response);
            }

            return response.StatusCode switch
            {
                404 => TypedResults.NotFound(response),
                409 => TypedResults.Conflict(response),
                _ => TypedResults.BadRequest(response),
            };
        }
        catch (Exception ex)
        {
            service.Logger.LogCritical("Critical failure: {ErrorMessage}", ex.Message);
            return TypedResults.InternalServerError();
        }
    }

    private static async Task<IResult> UpdateCategoryById(string categoryId, [FromBody] UpdateCategoryCommand command, ApplicationService service, IValidator<UpdateCategoryCommand> validator)
    {
        try
        {
            if (categoryId != command.Id)
            {
                return TypedResults.BadRequest("Category id mismatch");
            }

            var validation = await validator.ValidateAsync(command);
            if (!validation.IsValid)
            {
                var errors = validation.Errors.Select(x => x.ErrorMessage).ToList();
                var validationErrors = Result<UpdateCategoryCommand>.Failure(errors, 400);
                return TypedResults.BadRequest(validationErrors);
            }

            var response = await service.Mediator.Send(command);

            if (response.Success)
            {
                return TypedResults.NoContent();
            }

            return response.StatusCode switch
            {
                404 => TypedResults.NotFound(response),
                _ => TypedResults.BadRequest(response),
            };
        }
        catch (Exception ex)
        {
            service.Logger.LogCritical("Critical failure: {ErrorMessage}", ex.Message);
            return TypedResults.InternalServerError();
        }
    }

    private static async Task<IResult> DeleteCategoryById(string categoryId, ApplicationService service, IValidator<DeleteCategoryCommand> validator)
    {
        try
        {
            var command = new DeleteCategoryCommand(categoryId);
            var validation = await validator.ValidateAsync(command);
            if (!validation.IsValid)
            {
                var errors = validation.Errors.Select(x => x.ErrorMessage).ToList();
                var validationErrors = Result<DeleteCategoryCommand>.Failure(errors, 400);
                return TypedResults.BadRequest(validationErrors);
            }

            await service.Mediator.Send(command);

            return TypedResults.NoContent();
        }
        catch (Exception ex)
        {
            service.Logger.LogCritical("Critical failure: {ErrorMessage}", ex.Message);
            return TypedResults.InternalServerError();
        }
    }
}
