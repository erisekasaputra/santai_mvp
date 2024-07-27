using Catalog.API.Commands.Brands.CreateBrand;
using Catalog.API.Commands.Brands.DeleteBrand;
using Catalog.API.Commands.Brands.UpdateBrand;
using Catalog.API.DTOs.BrandDto;
using Catalog.API.Queries.Brands.GetBrandById;
using Catalog.API.Queries.Brands.GetBrandPaginated;
using Catalog.API.SeedWorks;
using Catalog.API.Services;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;

namespace Catalog.API.API;

public static class BrandAPI
{
    public static IEndpointRouteBuilder BrandRouter(this IEndpointRouteBuilder route, string groupName)
    {
        var app = route.MapGroup(groupName);

        app.MapGet("/brands/{brandId}", GetBrandById);
        app.MapGet("/brands", GetPaginatedBrand);
        app.MapPost("/brands", CreateNewBrand);
        app.MapPut("/brands/{brandId}", UpdateBrandById);
        app.MapDelete("/brands/{brandId}", DeleteBrandById);

        return app;
    }

    private static async Task<IResult> GetBrandById(
        string brandId,
        ApplicationService service,
        IValidator<GetBrandByIdQuery> validator)
    {
        try
        {
            var query = new GetBrandByIdQuery(brandId);

            var validation = await validator.ValidateAsync(query);

            if (!validation.IsValid)
            {
                var errors = validation.Errors.Select(x => x.ErrorMessage).ToList();
                var validationErrors = Result<BrandDto>.Failure(errors, 400);
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

    private static async Task<IResult> GetPaginatedBrand([AsParameters] GetBrandPaginatedQuery brandPaginatedQuery, ApplicationService service, IValidator<GetBrandPaginatedQuery> validator)
    {
        try
        {
            var validation = await validator.ValidateAsync(brandPaginatedQuery);

            if (!validation.IsValid)
            {
                var errors = validation.Errors.Select(x => x.ErrorMessage).ToList();
                var validationErrors = Result<BrandDto>.Failure(errors, 400);
                return TypedResults.BadRequest(validationErrors);
            }

            var response = await service.Mediator.Send(brandPaginatedQuery);

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

    private static async Task<IResult> CreateNewBrand(
       [FromBody] CreateBrandCommand command,
       ApplicationService service,
       IValidator<CreateBrandCommand> validator)
    {
        try
        {
            var validation = await validator.ValidateAsync(command);
            if (!validation.IsValid)
            {
                var errors = validation.Errors.Select(x => x.ErrorMessage).ToList();
                var validationErrors = Result<CreateBrandCommand>.Failure(errors, 400);
                return TypedResults.BadRequest(validationErrors);
            }

            var response = await service.Mediator.Send(command);

            if (response.Success)
            {
                response.WithLink(service.LinkGenerator.GetPathByName(nameof(GetBrandById)) ?? "");

                return TypedResults.Created(service.LinkGenerator.GetPathByName(nameof(GetBrandById)), response);
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

    private static async Task<IResult> UpdateBrandById(string brandId, [FromBody] UpdateBrandCommand command, ApplicationService service, IValidator<UpdateBrandCommand> validator)
    {
        try
        {
            if (brandId != command.Id)
            {
                return TypedResults.BadRequest("Item id mismatch");
            }

            var validation = await validator.ValidateAsync(command);
            if (!validation.IsValid)
            {
                var errors = validation.Errors.Select(x => x.ErrorMessage).ToList();
                var validationErrors = Result<UpdateBrandCommand>.Failure(errors, 400);
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

    private static async Task<IResult> DeleteBrandById(string brandId, ApplicationService service, IValidator<DeleteBrandCommand> validator)
    {
        try
        {
            var command = new DeleteBrandCommand(brandId);
            var validation = await validator.ValidateAsync(command);
            if (!validation.IsValid)
            {
                var errors = validation.Errors.Select(x => x.ErrorMessage).ToList();
                var validationErrors = Result<DeleteBrandCommand>.Failure(errors, 400);
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
