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

        app.MapGet("/items/{itemId}", GetItemById);
        app.MapGet("/items", GetPaginatedItem);
        
        app.MapPost("/items", CreateNewItem);
        
        app.MapPut("/items/{itemId}", UpdateItemById);

        app.MapDelete("/items/{itemId}", DeleteItemById);
        app.MapPatch("/items/{itemId}/undelete", UndeleteItemById);

        app.MapPatch("/items/{itemId}/activate", SetItemActivateById);
        app.MapPatch("/items/{itemId}/deactivate", SetItemDeactivateById);

        app.MapPatch("/items/stock/reduce", ReduceItemsStock);
        app.MapPatch("/items/stock/increase", IncreaseItemsStock);
        app.MapPatch("/items/stock", SetItemsStock);  
        app.MapPatch("/items/sold/reduce", ReduceItemsSold);
        app.MapPatch("/items/sold/increase", IncreaseItemsSold);
        app.MapPatch("/items/sold", SetItemsSold); 
        app.MapPatch("/items/price", SetItemsPrice); 
       
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
            return TypedResults.InternalServerError();
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
            return TypedResults.InternalServerError();
        }
    }

    private static async Task<IResult> CreateNewItem(
       [FromBody] CreateItemCommand command,
       ApplicationService service,
       IValidator<CreateItemCommand> validator,
       HttpContext httpContext)
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
                response.WithLink(service.LinkGenerator.GetPathByAction(httpContext, nameof(GetItemById)) ?? "");

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

    private static async Task<IResult> DeleteItemById(string itemId, ApplicationService service, IValidator<DeleteItemCommand> validator)
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
    
    private static async Task<IResult> UndeleteItemById(string itemId, ApplicationService service, IValidator<UndeleteItemCommand> validator)
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
    
    private static async Task<IResult> SetItemActivateById(string itemId, ApplicationService service, IValidator<ActivateItemCommand> validator)
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
    
    private static async Task<IResult> SetItemDeactivateById(string itemId, ApplicationService service, IValidator<DeactivateItemCommand> validator)
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

    private static async Task<IResult> ReduceItemsStock(ReduceItemStockQuantityCommand command, ApplicationService service, IValidator<ReduceItemStockQuantityCommand> validator)
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
    
    private static async Task<IResult> IncreaseItemsStock(AddItemStockQuantityCommand command, ApplicationService service, IValidator<AddItemStockQuantityCommand> validator)
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
    
    private static async Task<IResult> SetItemsStock(SetItemStockQuantityCommand command, ApplicationService service, IValidator<SetItemStockQuantityCommand> validator)
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
     

    private static async Task<IResult> ReduceItemsSold(ReduceItemSoldQuantityCommand command, ApplicationService service, IValidator<ReduceItemSoldQuantityCommand> validator)
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

    private static async Task<IResult> IncreaseItemsSold(AddItemSoldQuantityCommand command, ApplicationService service, IValidator<AddItemSoldQuantityCommand> validator)
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

    private static async Task<IResult> SetItemsSold(SetItemSoldQuantityCommand command, ApplicationService service, IValidator<SetItemSoldQuantityCommand> validator)
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

    private static async Task<IResult> SetItemsPrice(SetItemPriceCommand command, ApplicationService service, IValidator<SetItemPriceCommand> validator)
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
