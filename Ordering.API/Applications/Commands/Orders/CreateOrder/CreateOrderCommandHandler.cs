using Core.Enumerations;
using Core.Events;
using Core.Exceptions;
using Core.Messages;
using Core.Results;
using Core.Services.Interfaces;
using Core.Utilities;
using MassTransit;
using MediatR; 
using Ordering.API.Applications.Dtos.Requests;
using Ordering.API.Applications.Dtos.Responses;
using Ordering.API.Applications.Services.Interfaces;
using Ordering.API.Extensions;
using Ordering.Domain.Aggregates.OrderAggregate;
using Ordering.Domain.Aggregates.ScheduledOrderAggregate;
using Ordering.Domain.Enumerations;
using Ordering.Domain.SeedWork;
using Ordering.Domain.ValueObjects;
using System.Data; 
namespace Ordering.API.Applications.Commands.Orders.CreateOrder;
public class CreateOrderCommandHandler(
    ILogger<CreateOrderCommandHandler> logger,
    IPaymentService paymentService,
    IUnitOfWork unitOfWork,
    IAccountServiceAPI accountService,
    ICatalogServiceAPI catalogService,
    IPublishEndpoint publishEndpoint,
    IEncryptionService encryptionService) : IRequestHandler<CreateOrderCommand, Result>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly ILogger<CreateOrderCommandHandler> _logger = logger;
    private readonly IPaymentService _paymentService = paymentService; 
    private readonly IAccountServiceAPI _accountService = accountService; 
    private readonly ICatalogServiceAPI _catalogService = catalogService;
    private readonly IPublishEndpoint _publishEndpoint = publishEndpoint;
    private readonly IEncryptionService _encryptionService = encryptionService;


    public async Task<Result> Handle(CreateOrderCommand command, CancellationToken cancellationToken)
    {
        var requestItems = command.LineItems;
        await _unitOfWork.BeginTransactionAsync(IsolationLevel.ReadCommitted, cancellationToken); 
        try
        {  
            if (command.BuyerType is UserType.BusinessUser)
            { 
                (var buyer, var isAccountResponseSuccess) = await GetUserAccountAndFleetDetail<AccountIdentityBusinessUserResponseDto>(
                    command.BuyerId, 
                    command.Fleets); 

                (var isAccountValidationSuccess, var result) = await IsAccountResponseSuccess( 
                    isAccountResponseSuccess, 
                    buyer, 
                    cancellationToken); 

                if (!isAccountValidationSuccess)
                { 
                    return result;
                } 

                return await CreateOrderByRelatedUserType(
                    command,
                    buyer?.Data?.BusinessName,
                    buyer?.Data?.Fleets,
                    cancellationToken); 
            }
            else if (command.BuyerType is UserType.StaffUser)
            {
                (var buyer, var isAccountResponseSuccess) = await GetUserAccountAndFleetDetail<AccountIdentityStaffUserResponseDto>(
                    command.BuyerId, 
                    command.Fleets);

                (var isAccountValidationSuccess, var result) = await IsAccountResponseSuccess( 
                    isAccountResponseSuccess, 
                    buyer, 
                    cancellationToken);

                if (!isAccountValidationSuccess)
                {
                    return result;
                }
                return await CreateOrderByRelatedUserType(
                    command,
                    buyer?.Data?.Name,
                    buyer?.Data?.Fleets,
                    cancellationToken);  
            }
            else if (command.BuyerType is UserType.RegularUser)
            {
                (var buyer, var isAccountResponseSuccess) = await GetUserAccountAndFleetDetail<AccountIdentityRegularUserResponseDto>(
                    command.BuyerId, 
                    command.Fleets);

                (var isAccountValidationSuccess, var result) = await IsAccountResponseSuccess( 
                    isAccountResponseSuccess, 
                    buyer, 
                    cancellationToken);

                if (!isAccountValidationSuccess)
                {
                    return result;
                }

                return await CreateOrderByRelatedUserType(
                    command, 
                    buyer?.Data?.PersonalInfo.ToFullName,
                    buyer?.Data?.Fleets,
                    cancellationToken); 
            }

            return Result.Failure(
                   "Session user type with existing user in database does not match",
                   ResponseStatus.InternalServerError);
        }
        catch (AccountServiceHttpRequestException ex)
        { 
            LoggerHelper.LogError(_logger, ex);
            await RollbackAsync(cancellationToken);
            return Result.Failure(Messages.InternalServerError, ResponseStatus.InternalServerError);
        }
        catch (CatalogServiceHttpRequestException ex)
        {  
            LoggerHelper.LogError(_logger, ex);
            await RollbackAsync(cancellationToken);
            return Result.Failure(Messages.InternalServerError, ResponseStatus.InternalServerError);
        }
        catch (PaymentServiceHttpRequestException ex)
        {
            LoggerHelper.LogError(_logger, ex);
            await RollbackAndRecoveryStockAsync(requestItems, cancellationToken);
            return Result.Failure(Messages.InternalServerError, ResponseStatus.InternalServerError);
        }
        catch (ArgumentNullException ex)
        {
            await RollbackAndRecoveryStockAsync(requestItems, cancellationToken);
            return Result.Failure(ex.Message, ResponseStatus.BadRequest);
        }
        catch (DomainException ex)
        {
            await RollbackAndRecoveryStockAsync(requestItems, cancellationToken);
            return Result.Failure(ex.Message, ResponseStatus.BadRequest);
        }
        catch (NotImplementedException ex)
        {
            await RollbackAndRecoveryStockAsync(requestItems, cancellationToken); 
            LoggerHelper.LogError(_logger, ex);
            return Result.Failure(Messages.InternalServerError, ResponseStatus.InternalServerError);
        }
        catch (Exception ex)
        {
            await RollbackAndRecoveryStockAsync(requestItems, cancellationToken); 
            LoggerHelper.LogError(_logger, ex);
            return Result.Failure(Messages.InternalServerError, ResponseStatus.InternalServerError);
        }
    }

    private async Task<(bool isSuccess, Result accountResult)> IsAccountResponseSuccess<T>( 
        bool isAccountResponseSuccess,
        ResultResponseDto<T>? buyer,
        CancellationToken cancellationToken)
    { 
        if (!isAccountResponseSuccess)
        {
            if (buyer is null)
            {
                await RollbackAsync(cancellationToken); 
                return (false, Result.Failure(Messages.InternalServerError, ResponseStatus.InternalServerError));
            }

            await RollbackAsync(cancellationToken);
            return (false, Result.Failure(buyer.Message ?? Messages.UnknownError, ResponseStatus.BadRequest));
        }

        if (buyer is null)
        {
            await RollbackAsync(cancellationToken); 
            return (false, Result.Failure("User data not found", ResponseStatus.NotFound));
        }  

        return (true, Result.Success(null, ResponseStatus.Ok));
    }

    private async Task<(ResultResponseDto<T>? buyer, bool isAccountResponseSuccess)> GetUserAccountAndFleetDetail<T>(
        Guid buyerId, 
        IEnumerable<FleetRequest> requestFleets)
    {
        return await _accountService.GetUserDetail<T>(buyerId, requestFleets.Select(x => x.Id)); 
    }

    private async Task<Result> CreateOrderByRelatedUserType(
        CreateOrderCommand command,  
        string? buyerName,
        IEnumerable<AccountIdentityFleetResponseDto>? fleets,
        CancellationToken cancellationToken)
    {
        var lineItems = command.LineItems.Select(x => (x.Id, x.Quantity)); 
         
        if (fleets is null || !fleets.Any() || string.IsNullOrWhiteSpace(buyerName)) 
        { 
            await RollbackAsync(cancellationToken);
            return Result.Failure("Data not found for fleet or buyer name", ResponseStatus.NotFound);
        }

        (var items, var isCatalogResponseSuccess) = await _catalogService.SubstractStockAndGetDetailItems(lineItems);

        if (!isCatalogResponseSuccess)
        {
            if (items is null)
            {
                await RollbackAsync(cancellationToken);
                return Result.Failure(Messages.InternalServerError, ResponseStatus.InternalServerError);
            }

            await RollbackAsync(cancellationToken);
            return Result.Failure(items.Message ?? Messages.UnknownError, ResponseStatus.BadRequest)
                .WithData(items);
        }

        var order = new Order(  
            command.Currency,
            command.Address,
            command.Latitude,
            command.Longitude,
            command.BuyerId,
            buyerName,
            command.BuyerType,
            command.IsOrderScheduled,
            command.ScheduledOn); 

        order.SetSecretKey(
            await _encryptionService.EncryptAsync($"{order.Id}{order.Buyer.BuyerId}{order.GrandTotal.Amount}"));

        foreach (var lineItem in items?.Data ?? [])
        {
            if (IsNullCatalogItems(lineItem.Name, lineItem.Sku, lineItem.Price, lineItem.Currency))
            {
                await RollbackAndRecoveryStockAsync(command.LineItems, cancellationToken);
                return Result.Failure(
                    Messages.InternalServerError, ResponseStatus.InternalServerError);
            }

            order.AddOrderItem(
                new(lineItem.Id,
                    order.Id,
                    lineItem.Name!,
                    lineItem.Sku!,
                    lineItem.Price!.Value,
                    lineItem.Currency!.Value,
                    command.LineItems.First(x => x.Id == lineItem.Id).Quantity));
        }
  
        foreach (var fleet in fleets)
        {
            order.AddFleet(new(
                order.Id,
                fleet.Id,
                fleet.Brand,
                fleet.Model,
                fleet.RegistrationNumber,
                fleet.ImageUrl));
        }

        if (!string.IsNullOrWhiteSpace(command.CouponCode))
        {
            order.ApplyDiscount(Discount.CreateValueDiscount(order.Id, command.CouponCode, 10, command.Currency, 10));
        }

        order.ApplyTax(new Tax(10, command.Currency));

        order.ApplyFee(Fee.CreateByValue(order.Id, FeeDescription.MechanicFee, 10, command.Currency));

        order.ApplyFee(Fee.CreateByValue(order.Id, FeeDescription.ServiceFee, 10, command.Currency));

        order.CalculateGrandTotal();

        await _unitOfWork.Orders.CreateAsync(order, cancellationToken);

        if (order.IsShouldRequestPayment)
        {
            //await _paymentService.Checkout(order); 
        }

        if (order.IsScheduled)
        {
            var scheduledOrderWorker = new ScheduledOrder(order.Id, order.ScheduledOnUtc!.Value);
            await _unitOfWork.ScheduledOrders.CraeteAsync(scheduledOrderWorker);
        }

        var orderDto = order.ToOrderResponseDto();

        await _unitOfWork.CommitTransactionAsync(cancellationToken);

        return Result.Success(orderDto, ResponseStatus.Created);
    }

    private static bool IsNullCatalogItems(string? name, string? sku, decimal? price, Currency? currency)
    {
        if (string.IsNullOrWhiteSpace(name)) return true;
        if (string.IsNullOrWhiteSpace(sku)) return true;
        if (price is null || !price.HasValue) return true;
        if (currency is null || !currency.HasValue) return true;  
        return false;
    }

    private async Task RollbackAndRecoveryStockAsync(IEnumerable<LineItemRequest> items, CancellationToken cancellationToken)
    {
        await _unitOfWork.RollbackTransactionAsync(cancellationToken); 

        await _unitOfWork.BeginTransactionAsync(IsolationLevel.ReadCommitted, cancellationToken); 

        await _publishEndpoint.Publish(
            new OrderFailedRecoveryStockIntegrationEvent(
                items.Select(x => new CatalogItemStockIntegrationEvent(x.Id, x.Quantity))),
            cancellationToken);

        await _unitOfWork.CommitTransactionAsync(cancellationToken);
    }

    private async Task RollbackAsync(CancellationToken cancellationToken)
    {
        await _unitOfWork.RollbackTransactionAsync(cancellationToken);
    }
}
