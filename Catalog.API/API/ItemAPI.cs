using Catalog.API.Applications.Commands.Items.AddSold;
using Catalog.API.Applications.Commands.Items.AddStock;
using Catalog.API.Applications.Commands.Items.CreateItem;
using Catalog.API.Applications.Commands.Items.DeleteItem;
using Catalog.API.Applications.Commands.Items.ReduceSold;
using Catalog.API.Applications.Commands.Items.ReduceStock;
using Catalog.API.Applications.Commands.Items.SetPrice;
using Catalog.API.Applications.Commands.Items.SetSold;
using Catalog.API.Applications.Commands.Items.SetStock;
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

        app.MapPatch("/items/stock/reduce", ReduceStocks);
        app.MapPatch("/items/stock/increase", IncreaseStocks);
        app.MapPatch("/items/stock", SetStocks);
        
        app.MapPatch("/items/sold/reduce", ReduceSolds);
        app.MapPatch("/items/sold/increase", IncreaseSolds);
        app.MapPatch("/items/sold", SetSolds);

        app.MapPatch("/items/price", SetPrices);

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
                var errors = validation.Errors.Select(x => x.ErrorMessage).ToList();
                var validationErrors = Result<CreateItemCommand>.Failure(errors, 400);
                return TypedResults.BadRequest(validationErrors);
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
            return TypedResults.InternalServerError();
        }
    }

    private static async Task<IResult> ReduceStocks(ReduceStockCommand command, ApplicationService service, IValidator<ReduceStockCommand> validator)
    {
        try
        { 
            var validation = await validator.ValidateAsync(command);
            if (!validation.IsValid)
            {
                var errors = validation.Errors.Select(x => x.ErrorMessage).ToList();
                var validationErrors = Result<ReduceStockCommand>.Failure(errors, 400);
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
    
    private static async Task<IResult> IncreaseStocks(AddStockCommand command, ApplicationService service, IValidator<AddStockCommand> validator)
    {
        try
        { 
            var validation = await validator.ValidateAsync(command);
            if (!validation.IsValid)
            {
                var errors = validation.Errors.Select(x => x.ErrorMessage).ToList();
                var validationErrors = Result<AddStockCommand>.Failure(errors, 400);
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
    
    private static async Task<IResult> SetStocks(SetStockCommand command, ApplicationService service, IValidator<SetStockCommand> validator)
    {
        try
        { 
            var validation = await validator.ValidateAsync(command);
            if (!validation.IsValid)
            {
                var errors = validation.Errors.Select(x => x.ErrorMessage).ToList();
                var validationErrors = Result<SetStockCommand>.Failure(errors, 400);
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





    private static async Task<IResult> ReduceSolds(ReduceSoldCommand command, ApplicationService service, IValidator<ReduceSoldCommand> validator)
    {
        try
        {
            var validation = await validator.ValidateAsync(command);
            if (!validation.IsValid)
            {
                var errors = validation.Errors.Select(x => x.ErrorMessage).ToList();
                var validationErrors = Result<ReduceSoldCommand>.Failure(errors, 400);
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

    private static async Task<IResult> IncreaseSolds(AddSoldCommand command, ApplicationService service, IValidator<AddSoldCommand> validator)
    {
        try
        {
            var validation = await validator.ValidateAsync(command);
            if (!validation.IsValid)
            {
                var errors = validation.Errors.Select(x => x.ErrorMessage).ToList();
                var validationErrors = Result<AddSoldCommand>.Failure(errors, 400);
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

    private static async Task<IResult> SetSolds(SetSoldCommand command, ApplicationService service, IValidator<SetSoldCommand> validator)
    {
        try
        {
            var validation = await validator.ValidateAsync(command);
            if (!validation.IsValid)
            {
                var errors = validation.Errors.Select(x => x.ErrorMessage).ToList();
                var validationErrors = Result<SetSoldCommand>.Failure(errors, 400);
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


    private static async Task<IResult> SetPrices(SetPriceCommand command, ApplicationService service, IValidator<SetPriceCommand> validator)
    {
        try
        {
            var validation = await validator.ValidateAsync(command);
            if (!validation.IsValid)
            {
                var errors = validation.Errors.Select(x => x.ErrorMessage).ToList();
                var validationErrors = Result<SetPriceCommand>.Failure(errors, 400);
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
} 
