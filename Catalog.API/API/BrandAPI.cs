using Catalog.API.Applications.Commands.Brands.CreateBrand;
using Catalog.API.Applications.Commands.Brands.DeleteBrand;
using Catalog.API.Applications.Commands.Brands.UpdateBrand;
using Catalog.API.Applications.Queries.Brands.GetBrandById;
using Catalog.API.Applications.Queries.Brands.GetBrandPaginated;
using Catalog.API.Applications.Services;
using Catalog.API.Extensions;
using Core.Dtos;
using Core.SeedWorks;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;

namespace Catalog.API.API;

public static class BrandAPI
{
    const int _cacheExpiry = 10;
    public static IEndpointRouteBuilder BrandRouter(this IEndpointRouteBuilder route, string groupName)
    {
        var app = route.MapGroup(groupName);

        app.MapGet("/brands/{brandId}", GetBrandById)
            .RequireAuthorization()
            .CacheOutput(config => config.Expire(TimeSpan.FromSeconds(_cacheExpiry)));

        app.MapGet("/brands", GetPaginatedBrand)
            .RequireAuthorization()
            .CacheOutput(config =>
            {
                config.Expire(TimeSpan.FromSeconds(_cacheExpiry));
                config.SetVaryByQuery(PaginatedRequestDto.PageNumberName, PaginatedRequestDto.PageSizeName);
            }); 

        app.MapPost("/brands", CreateNewBrand)
            .RequireAuthorization(PolicyName.AdministratorPolicy.ToString());

        app.MapPut("/brands/{brandId}", UpdateBrandById)
            .RequireAuthorization(PolicyName.AdministratorPolicy.ToString());

        app.MapDelete("/brands/{brandId}", DeleteBrandById)
            .RequireAuthorization(PolicyName.AdministratorPolicy.ToString());

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

            return response.ToIResult();
        }
        catch (Exception ex)
        {
            service.Logger.LogCritical("Critical failure: {ErrorMessage}", ex.Message);
            return TypedResults.InternalServerError();
        }
    }

    private static async Task<IResult> GetPaginatedBrand(
        [AsParameters] PaginatedRequestDto paginatedRequest,
        [FromServices] ApplicationService service, 
        [FromServices] IValidator<GetBrandPaginatedQuery> validator)
    {
        try
        {
            var paginatedQuery = new GetBrandPaginatedQuery(paginatedRequest.PageNumber, paginatedRequest.PageSize);

            var validation = await validator.ValidateAsync(paginatedQuery);

            if (!validation.IsValid)
            { 
                return TypedResults.BadRequest(validation.Errors);
            }

            var response = await service.Mediator.Send(paginatedQuery);

            return response.ToIResult();
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
             
            return response.ToIResult();
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

            return response.ToIResult();
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
