using Catalog.API.Applications.Commands.Items.ActivateItem;
using Catalog.API.Applications.Commands.Items.AddItemSoldQuantity;
using Catalog.API.Applications.Commands.Items.AddItemStockQuantity; 
using Catalog.API.Applications.Commands.Items.CreateItem;
using Catalog.API.Applications.Commands.Items.DeactivateItem;
using Catalog.API.Applications.Commands.Items.DeleteItem;
using Catalog.API.Applications.Commands.Items.ReduceItemSoldQuantity;
using Catalog.API.Applications.Commands.Items.ReduceItemStockQuantity; 
using Catalog.API.Applications.Commands.Items.SetItemPrice;
using Catalog.API.Applications.Commands.Items.SetItemSoldQuantity;
using Catalog.API.Applications.Commands.Items.SetItemStockQuantity;
using Catalog.API.Applications.Commands.Items.UndeleteItem;
using Catalog.API.Applications.Commands.Items.UpdateItem;
using Catalog.API.Applications.Queries.Items.GetItemById;
using Catalog.API.Applications.Queries.Items.GetItemPaginated;
using Catalog.API.DTOs.Item;
using Catalog.API.SeedWork;
using Catalog.API.Services;
using FluentValidation;
using Microsoft.AspNetCore.Mvc; 
namespace Catalog.API.API;

public static class ItemAPI
{
    public static IEndpointRouteBuilder ItemRouter(this IEndpointRouteBuilder route, string groupName)
    {
        var app = route.MapGroup(groupName);

        app.MapGet("/items/{itemId}", GetItemById).RequireAuthorization();
        app.MapGet("/items", GetPaginatedItem).RequireAuthorization();

        app.MapPost("/items", CreateNewItem).RequireAuthorization();

        app.MapPut("/items/{itemId}", UpdateItemById).RequireAuthorization();

        app.MapDelete("/items/{itemId}", DeleteItemById).RequireAuthorization();
        app.MapPatch("/items/{itemId}/undelete", UndeleteItemById).RequireAuthorization();

        app.MapPatch("/items/{itemId}/activate", SetItemActivateById).RequireAuthorization();
        app.MapPatch("/items/{itemId}/deactivate", SetItemDeactivateById).RequireAuthorization();

        app.MapPatch("/items/stock/reduce", ReduceItemsStock).RequireAuthorization();
        app.MapPatch("/items/stock/increase", IncreaseItemsStock).RequireAuthorization();
        app.MapPatch("/items/stock", SetItemsStock).RequireAuthorization();
        app.MapPatch("/items/sold/reduce", ReduceItemsSold).RequireAuthorization();
        app.MapPatch("/items/sold/increase", IncreaseItemsSold).RequireAuthorization();
        app.MapPatch("/items/sold", SetItemsSold).RequireAuthorization();
        app.MapPatch("/items/price", SetItemsPrice).RequireAuthorization();

        return app;
    }
    private static async Task<IResult> GetItemById(
        Guid itemId,
        [FromServices] ApplicationService service,
        [FromServices] IValidator<GetItemByIdQuery> validator)
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
            return TypedResults.InternalServerError();
        }
    }

    private static async Task<IResult> GetPaginatedItem(
        [AsParameters] GetItemPaginatedQuery itemPaginatedQuery, 
        [FromServices] ApplicationService service,
        [FromServices] IValidator<GetItemPaginatedQuery> validator)
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
            return TypedResults.InternalServerError();
        }
    }

    private static async Task<IResult> CreateNewItem(
       [FromBody] CreateItemCommand command,
       [FromServices] ApplicationService service,
       [FromServices] IValidator<CreateItemCommand> validator)
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
            return TypedResults.InternalServerError();
        }
    }

    private static async Task<IResult> UpdateItemById(
        Guid itemId,
        [FromBody] UpdateItemCommand command,
        [FromServices] ApplicationService service,
        [FromServices] IValidator<UpdateItemCommand> validator)
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

    private static async Task<IResult> DeleteItemById(
        Guid itemId,
        [FromServices] ApplicationService service,
        [FromServices] IValidator<DeleteItemCommand> validator)
    {
        try
        {
            var command = new DeleteItemCommand(itemId);
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

    private static async Task<IResult> UndeleteItemById(
        Guid itemId,
        [FromServices] ApplicationService service,
        [FromServices] IValidator<UndeleteItemCommand> validator)
    {
        try
        {
            var command = new UndeleteItemCommand(itemId);
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

    private static async Task<IResult> SetItemActivateById(
        Guid itemId,
        [FromServices] ApplicationService service,
        [FromServices] IValidator<ActivateItemCommand> validator)
    {
        try
        {
            var command = new ActivateItemCommand(itemId);
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

    private static async Task<IResult> SetItemDeactivateById(
        Guid itemId,
        [FromServices] ApplicationService service,
        [FromServices] IValidator<DeactivateItemCommand> validator)
    {
        try
        {
            var command = new DeactivateItemCommand(itemId);
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

    private static async Task<IResult> ReduceItemsStock(
        [FromBody] ReduceItemStockQuantityCommand command,
        [FromServices] ApplicationService service,
        [FromServices] IValidator<ReduceItemStockQuantityCommand> validator)
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

    private static async Task<IResult> IncreaseItemsStock(
        [FromBody] AddItemStockQuantityCommand command,
        [FromServices] ApplicationService service,
        [FromServices] IValidator<AddItemStockQuantityCommand> validator)
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
    
    private static async Task<IResult> SetItemsStock(
        [FromBody] SetItemStockQuantityCommand command,
        [FromServices] ApplicationService service,
        [FromServices] IValidator<SetItemStockQuantityCommand> validator)
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
     

    private static async Task<IResult> ReduceItemsSold(
        [FromBody] ReduceItemSoldQuantityCommand command,
        [FromServices] ApplicationService service,
        [FromServices] IValidator<ReduceItemSoldQuantityCommand> validator)
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

    private static async Task<IResult> IncreaseItemsSold(
        [FromBody] AddItemSoldQuantityCommand command,
        [FromServices] ApplicationService service,
        [FromServices] IValidator<AddItemSoldQuantityCommand> validator)
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

    private static async Task<IResult> SetItemsSold(
        [FromBody] SetItemSoldQuantityCommand command,
        [FromServices] ApplicationService service,
        [FromServices] IValidator<SetItemSoldQuantityCommand> validator)
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

    private static async Task<IResult> SetItemsPrice(
        [FromBody] SetItemPriceCommand command,
        [FromServices] ApplicationService service,
        [FromServices] IValidator<SetItemPriceCommand> validator)
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
} 
