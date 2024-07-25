using Catalog.API.Commands.CreateItem;
using Catalog.API.Commands.DeleteItem;
using Catalog.API.Commands.UpdateItem;
using Catalog.API.DTOs.ItemDto;
using Catalog.API.Queries.GetItemById;
using Catalog.API.Queries.GetItemPaginated;
using Catalog.API.SeedWorks;
using Catalog.API.Services;
using FluentValidation;
using Microsoft.AspNetCore.Mvc; 
namespace Catalog.API.API;

public static class CatalogAPI
{
    public static IEndpointRouteBuilder CatalogRouter(this IEndpointRouteBuilder route, string groupName)
    {
        var app = route.MapGroup(groupName);
         
        app.MapGet("/items/{itemId}", GetItemById).WithName(nameof(GetItemById)).WithTags(nameof(GetItemById));
        app.MapGet("/items", GetPaginatedItem).WithName(nameof(GetPaginatedItem)).WithTags(nameof(GetPaginatedItem));
        app.MapPost("/items", CreateNewItem).WithName(nameof(CreateNewItem)).WithTags(nameof(CreateNewItem));
        app.MapPut("/items/{itemId}", UpdateItemById).WithName(nameof(UpdateItemById)).WithTags(nameof(UpdateItemById));
        app.MapDelete("/items/{itemId}", DeleteItemById).WithName(nameof(DeleteItemById)).WithTags(nameof(DeleteItemById));

        return app;
    } 
    private static async Task<IResult> GetItemById(
        string itemId,
        ApplicationService service,
        IValidator<GetItemByIdQuery> validator)
    {
        try
        {
            var query = new GetItemByIdQuery(itemId);

            var validation = await validator.ValidateAsync(query);

            if (!validation.IsValid)
            {
                var errors = validation.Errors.Select(x => x.ErrorMessage).ToList();
                var validationErrors = Result<ItemDto>.Failure(errors, 400);
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
            return TypedResults.BadRequest();
        } 
    } 

    private static async Task<IResult> GetPaginatedItem([AsParameters] GetItemPaginatedQuery itemPaginatedQuery, ApplicationService service, IValidator<GetItemPaginatedQuery> validator)
    {
        try
        {
            var validation = await validator.ValidateAsync(itemPaginatedQuery);

            if (!validation.IsValid)
            {
                var errors = validation.Errors.Select(x => x.ErrorMessage).ToList();
                var validationErrors = Result<ItemDto>.Failure(errors, 400);
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
            return TypedResults.BadRequest();
        } 
    }

    private static async Task<IResult> CreateNewItem(
       [FromBody] CreateItemCommand command,
       ApplicationService service,
       IValidator<CreateItemCommand> validator)
    {
        try
        {
            var validation = await validator.ValidateAsync(command);
            if (!validation.IsValid)
            {
                var errors = validation.Errors.Select(x => x.ErrorMessage).ToList();
                var validationErrors = Result<CreateItemCommand>.Failure(errors, 400);
                return TypedResults.BadRequest(validationErrors);
            }

            var response = await service.Mediator.Send(command);

            if (response.Success)
            {
                response.WithLink(service.LinkGenerator.GetPathByName(nameof(GetItemById)) ?? "");

                return TypedResults.Created(service.LinkGenerator.GetPathByName(nameof(GetItemById)), response);
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
            return TypedResults.BadRequest();
        }
    }

    private static async Task<IResult> UpdateItemById(string itemId, [FromBody] UpdateItemCommand command, ApplicationService service, IValidator<UpdateItemCommand> validator)
    {
        try
        {
            if (itemId != command.Id)
            {
                return TypedResults.BadRequest("Item id mismatch");
            }

            var validation = await validator.ValidateAsync(command);
            if (!validation.IsValid)
            {
                var errors = validation.Errors.Select(x => x.ErrorMessage).ToList();
                var validationErrors = Result<UpdateItemCommand>.Failure(errors, 400);
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
            return TypedResults.BadRequest();
        }
    }
    
    private static async Task<IResult> DeleteItemById(string itemId, ApplicationService service, IValidator<DeleteItemCommand> validator)
    {
        try
        {
            var command = new DeleteItemCommand(itemId);
            var validation = await validator.ValidateAsync(command);
            if (!validation.IsValid)
            {
                var errors = validation.Errors.Select(x => x.ErrorMessage).ToList();
                var validationErrors = Result<DeleteItemCommand>.Failure(errors, 400);
                return TypedResults.BadRequest(validationErrors);
            }

            await service.Mediator.Send(command);

            return TypedResults.NoContent();
        }
        catch (Exception ex)
        {
            service.Logger.LogCritical("Critical failure: {ErrorMessage}", ex.Message);
            return TypedResults.BadRequest();
        }
    }
} 
