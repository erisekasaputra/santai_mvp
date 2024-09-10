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
using Catalog.API.Applications.Services;
using Catalog.API.Extensions;
using Core.Dtos;
using Core.Messages; 
using Core.SeedWorks;
using Core.Services.Interfaces;
using FluentValidation;
using Microsoft.AspNetCore.Mvc; 
namespace Catalog.API.API;

public static class ItemAPI
{
    const int _cacheExpiry = 10;
    public static IEndpointRouteBuilder ItemRouter(this IEndpointRouteBuilder route, string groupName)
    {
        var app = route.MapGroup(groupName); 

        app.MapGet("/items/{itemId}", GetItemById)
            .RequireAuthorization()
            .CacheOutput(config => config.Expire(TimeSpan.FromSeconds(_cacheExpiry)));

        app.MapGet("/items", GetPaginatedItem)
            .RequireAuthorization().CacheOutput(config =>
            {
                config.Expire(TimeSpan.FromSeconds(_cacheExpiry));
                config.SetVaryByQuery(PaginatedRequestDto.PageNumberName, PaginatedRequestDto.PageSizeName);
            }); 

        app.MapPost("/items", CreateNewItem)
            .RequireAuthorization(PolicyName.AdministratorUserOnlyPolicy.ToString());

        app.MapPut("/items/{itemId}", UpdateItemById)
            .RequireAuthorization(PolicyName.AdministratorUserOnlyPolicy.ToString());

        app.MapDelete("/items/{itemId}", DeleteItemById)
            .RequireAuthorization(PolicyName.AdministratorUserOnlyPolicy.ToString());

        app.MapPatch("/items/{itemId}/undelete", UndeleteItemById)
            .RequireAuthorization(PolicyName.AdministratorUserOnlyPolicy.ToString());

        app.MapPatch("/items/{itemId}/activate", SetItemActivateById)
            .RequireAuthorization(PolicyName.AdministratorUserOnlyPolicy.ToString());

        app.MapPatch("/items/{itemId}/deactivate", SetItemDeactivateById)
            .RequireAuthorization(PolicyName.AdministratorUserOnlyPolicy.ToString());

        app.MapPatch("/items/stock/reduce", ReduceItemsStock)
            .RequireAuthorization(PolicyName.ServiceToServiceAndAdministratorUserPolicy.ToString());

        app.MapPatch("/items/stock/increase", IncreaseItemsStock)
            .RequireAuthorization(PolicyName.ServiceToServiceAndAdministratorUserPolicy.ToString());

        app.MapPatch("/items/stock", SetItemsStock)
            .RequireAuthorization(PolicyName.ServiceToServiceAndAdministratorUserPolicy.ToString());

        app.MapPatch("/items/sold/reduce", ReduceItemsSold)
            .RequireAuthorization(PolicyName.ServiceToServiceAndAdministratorUserPolicy.ToString());

        app.MapPatch("/items/sold/increase", IncreaseItemsSold)
            .RequireAuthorization(PolicyName.ServiceToServiceAndAdministratorUserPolicy.ToString());

        app.MapPatch("/items/sold", SetItemsSold)
            .RequireAuthorization(PolicyName.ServiceToServiceAndAdministratorUserPolicy.ToString());

        app.MapPatch("/items/price", SetItemsPrice)
            .RequireAuthorization(PolicyName.AdministratorUserOnlyPolicy.ToString());

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
                return TypedResults.BadRequest(validation.Errors);
            }

            var response = await service.Mediator.Send(query); 

            return response.ToIResult();
        }
        catch (Exception ex)
        {
            service.Logger.LogCritical("Critical failure: {ErrorMessage}", ex.Message);
            return TypedResults.InternalServerError(Messages.InternalServerError);
        }
    }

    private static async Task<IResult> GetPaginatedItem(
        [AsParameters] PaginatedRequestDto paginatedRequest, 
        [FromServices] ApplicationService service,
        [FromServices] IValidator<GetItemPaginatedQuery> validator)
    {
        try
        {
            var query = new GetItemPaginatedQuery(paginatedRequest.PageNumber, paginatedRequest.PageSize);

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
            return TypedResults.InternalServerError(Messages.InternalServerError);
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


            return response.ToIResult();
        }
        catch (Exception ex)
        {
            service.Logger.LogCritical("Critical failure: {ErrorMessage}", ex.Message);
            return TypedResults.InternalServerError(Messages.InternalServerError);
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


            return response.ToIResult();
        }
        catch (Exception ex)
        {
            service.Logger.LogCritical("Critical failure: {ErrorMessage}", ex.Message);
            return TypedResults.InternalServerError(Messages.InternalServerError);
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

            var response = await service.Mediator.Send(command);
             
            return response.ToIResult();
        }
        catch (Exception ex)
        {
            service.Logger.LogCritical("Critical failure: {ErrorMessage}", ex.Message);
            return TypedResults.InternalServerError(Messages.InternalServerError);
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

            var response = await service.Mediator.Send(command); 

            return response.ToIResult();
        }
        catch (Exception ex)
        {
            service.Logger.LogCritical("Critical failure: {ErrorMessage}", ex.Message);
            return TypedResults.InternalServerError(Messages.InternalServerError);
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

            var response = await service.Mediator.Send(command); 

            return response.ToIResult();
        }
        catch (Exception ex)
        {
            service.Logger.LogCritical("Critical failure: {ErrorMessage}", ex.Message);
            return TypedResults.InternalServerError(Messages.InternalServerError);
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

            var response = await service.Mediator.Send(command); 

            return response.ToIResult();
        }
        catch (Exception ex)
        {
            service.Logger.LogCritical("Critical failure: {ErrorMessage}", ex.Message);
            return TypedResults.InternalServerError(Messages.InternalServerError);
        }
    }

    private static async Task<IResult> ReduceItemsStock(
        [FromBody] ReduceItemStockQuantityCommand command,
        [FromServices] ApplicationService service,
        [FromServices] IValidator<ReduceItemStockQuantityCommand> validator,
        [FromServices] IUserInfoService userInfoService)
    {
        try
        {
            var claim = userInfoService.GetUserInfoAsync();

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
            return TypedResults.InternalServerError(Messages.InternalServerError);
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

            return response.ToIResult();
        }
        catch (Exception ex)
        {
            service.Logger.LogCritical("Critical failure: {ErrorMessage}", ex.Message);
            return TypedResults.InternalServerError(Messages.InternalServerError);
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

            return response.ToIResult();
        }
        catch (Exception ex)
        {
            service.Logger.LogCritical("Critical failure: {ErrorMessage}", ex.Message);
            return TypedResults.InternalServerError(Messages.InternalServerError);
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

            return response.ToIResult();
        }
        catch (Exception ex)
        {
            service.Logger.LogCritical("Critical failure: {ErrorMessage}", ex.Message);
            return TypedResults.InternalServerError(Messages.InternalServerError);
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

            return response.ToIResult();
        }
        catch (Exception ex)
        {
            service.Logger.LogCritical("Critical failure: {ErrorMessage}", ex.Message);
            return TypedResults.InternalServerError(Messages.InternalServerError);
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

            return response.ToIResult();
        }
        catch (Exception ex)
        {
            service.Logger.LogCritical("Critical failure: {ErrorMessage}", ex.Message);
            return TypedResults.InternalServerError(Messages.InternalServerError);
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

            return response.ToIResult();
        }
        catch (Exception ex)
        {
            service.Logger.LogCritical("Critical failure: {ErrorMessage}", ex.Message);
            return TypedResults.InternalServerError(Messages.InternalServerError);
        }
    }
} 
