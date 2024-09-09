using Catalog.API.Applications.Commands.Categories.CreateCategory;
using Catalog.API.Applications.Commands.Categories.DeleteCategory;
using Catalog.API.Applications.Commands.Categories.UpdateCategory;
using Catalog.API.Applications.Queries.Categories.GetCategoryById;
using Catalog.API.Applications.Queries.Categories.GetCategoryPaginated;
using Catalog.API.Applications.Services;
using Catalog.API.Extensions;
using Core.Dtos;
using Core.SeedWorks;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;

namespace Catalog.API.API;

public static class CategoryAPI
{
    const int _cacheExpiry = 10;
    public static IEndpointRouteBuilder CategoryRouter(this IEndpointRouteBuilder route, string groupName)
    {
        var app = route.MapGroup(groupName);

        app.MapGet("/categories/{categoryId}", GetCategoryById)
            .RequireAuthorization()
            .CacheOutput(config => config.Expire(TimeSpan.FromSeconds(_cacheExpiry)));

        app.MapGet("/categories", GetPaginatedCategory)
            .RequireAuthorization()
            .CacheOutput(config =>
            {
                config.Expire(TimeSpan.FromSeconds(_cacheExpiry));
                config.SetVaryByQuery(PaginatedRequestDto.PageNumberName, PaginatedRequestDto.PageSizeName);
            });

        app.MapPost("/categories", CreateNewCategory)
            .RequireAuthorization(PolicyName.AdministratorPolicy);

        app.MapPut("/categories/{categoryId}", UpdateCategoryById)
            .RequireAuthorization(PolicyName.AdministratorPolicy);

        app.MapDelete("/categories/{categoryId}", DeleteCategoryById)
            .RequireAuthorization(PolicyName.AdministratorPolicy);

        return app;
    }

    private static async Task<IResult> GetCategoryById(
        Guid categoryId,
        [FromServices] ApplicationService service,
        [FromServices] IValidator<GetCategoryByIdQuery> validator)
    {
        try
        {
            var query = new GetCategoryByIdQuery(categoryId);

            var validation = await validator.ValidateAsync(query);

            if (!validation.IsValid)
            { 
                return TypedResults.BadRequest(validation.Errors);
            }

            var response = await service.Mediator.Send(query); 

            return response.ToIResult();
        }
        catch (Exception ex)
        {
            service.Logger.LogCritical("Critical failure: {ErrorMessage}", ex.Message);
            return TypedResults.InternalServerError();
        }
    }

    private static async Task<IResult> GetPaginatedCategory(
        [AsParameters] PaginatedRequestDto request, 
        [FromServices] ApplicationService service, 
        [FromServices] IValidator<GetCategoryPaginatedQuery> validator)
    {
        try
        {
            var query = new GetCategoryPaginatedQuery(request.PageNumber, request.PageSize);
            var validation = await validator.ValidateAsync(query);

            if (!validation.IsValid)
            { 
                return TypedResults.BadRequest(validation.Errors);
            }

            var response = await service.Mediator.Send(query); 

            return response.ToIResult();
        }
        catch (Exception ex)
        {
            service.Logger.LogCritical("Critical failure: {ErrorMessage}", ex.Message);
            return TypedResults.InternalServerError();
        }
    }

    private static async Task<IResult> CreateNewCategory(
       [FromBody] CreateCategoryCommand command,
       [FromServices] ApplicationService service,
       [FromServices] IValidator<CreateCategoryCommand> validator)
    {
        try
        {
            var validation = await validator.ValidateAsync(command);
            if (!validation.IsValid)
            { 
                return TypedResults.BadRequest(validation.Errors);
            }

            var response = await service.Mediator.Send(command); 

            return response.ToIResult(); 
        }
        catch (Exception ex)
        {
            service.Logger.LogCritical("Critical failure: {ErrorMessage}", ex.Message);
            return TypedResults.InternalServerError();
        }
    }

    private static async Task<IResult> UpdateCategoryById(
        Guid categoryId, 
        [FromBody] UpdateCategoryCommand command, 
        [FromServices] ApplicationService service, 
        [FromServices] IValidator<UpdateCategoryCommand> validator)
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
                return TypedResults.BadRequest(validation.Errors);
            }

            var response = await service.Mediator.Send(command); 

            return response.ToIResult();
        }
        catch (Exception ex)
        {
            service.Logger.LogCritical("Critical failure: {ErrorMessage}", ex.Message);
            return TypedResults.InternalServerError();
        }
    }

    private static async Task<IResult> DeleteCategoryById(
        Guid categoryId, 
        [FromServices] ApplicationService service, 
        [FromServices] IValidator<DeleteCategoryCommand> validator)
    {
        try
        {
            var command = new DeleteCategoryCommand(categoryId);
            var validation = await validator.ValidateAsync(command);
            if (!validation.IsValid)
            { 
                return TypedResults.BadRequest(validation.Errors);
            }

            var response = await service.Mediator.Send(command); 

            return response.ToIResult();
        }
        catch (Exception ex)
        {
            service.Logger.LogCritical("Critical failure: {ErrorMessage}", ex.Message);
            return TypedResults.InternalServerError();
        }
    }
}
