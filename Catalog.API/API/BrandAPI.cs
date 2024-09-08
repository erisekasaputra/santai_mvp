using Catalog.API.Applications.Commands.Brands.CreateBrand;
using Catalog.API.Applications.Commands.Brands.DeleteBrand;
using Catalog.API.Applications.Commands.Brands.UpdateBrand;
using Catalog.API.Applications.Queries.Brands.GetBrandById;
using Catalog.API.Applications.Queries.Brands.GetBrandPaginated; 
using Catalog.API.Services;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;

namespace Catalog.API.API;

public static class BrandAPI
{
    public static IEndpointRouteBuilder BrandRouter(this IEndpointRouteBuilder route, string groupName)
    {
        var app = route.MapGroup(groupName);

        app.MapGet("/brands/{brandId}", GetBrandById).RequireAuthorization();
        app.MapGet("/brands", GetPaginatedBrand).RequireAuthorization();
        app.MapPost("/brands", CreateNewBrand).RequireAuthorization();
        app.MapPut("/brands/{brandId}", UpdateBrandById).RequireAuthorization();
        app.MapDelete("/brands/{brandId}", DeleteBrandById).RequireAuthorization();

        return app;
    }

    private static async Task<IResult> GetBrandById(
        Guid brandId,
        [FromServices] ApplicationService service,
        [FromServices] IValidator<GetBrandByIdQuery> validator)
    {
        try
        {
            var query = new GetBrandByIdQuery(brandId);

            var validation = await validator.ValidateAsync(query);

            if (!validation.IsValid)
            {  
                return TypedResults.BadRequest(validation.Errors);
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

    private static async Task<IResult> GetPaginatedBrand(
        [AsParameters] GetBrandPaginatedQuery brandPaginatedQuery,
        [FromServices] ApplicationService service, 
        [FromServices] IValidator<GetBrandPaginatedQuery> validator)
    {
        try
        {
            var validation = await validator.ValidateAsync(brandPaginatedQuery);

            if (!validation.IsValid)
            { 
                return TypedResults.BadRequest(validation.Errors);
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
       [FromServices] ApplicationService service,
       [FromServices] IValidator<CreateBrandCommand> validator)
    {
        try
        {
            var validation = await validator.ValidateAsync(command);
            if (!validation.IsValid)
            { 
                return TypedResults.BadRequest(validation.Errors);
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

    private static async Task<IResult> UpdateBrandById(
        Guid brandId, 
        [FromBody] UpdateBrandCommand command,
        [FromServices] ApplicationService service, 
        [FromServices] IValidator<UpdateBrandCommand> validator)
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
                return TypedResults.BadRequest(validation.Errors);
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

    private static async Task<IResult> DeleteBrandById(
        Guid brandId, 
        [FromServices] ApplicationService service, 
        [FromServices] IValidator<DeleteBrandCommand> validator)
    {
        try
        {
            var command = new DeleteBrandCommand(brandId);
            var validation = await validator.ValidateAsync(command);
            if (!validation.IsValid)
            { 
                return TypedResults.BadRequest(validation.Errors);
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
