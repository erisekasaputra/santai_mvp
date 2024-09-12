using Core.Enumerations;
using Core.Events;
using Core.Exceptions;
using Core.Messages;
using Core.Results;
using Core.Utilities;
using MassTransit;
using MediatR;
using Microsoft.IdentityModel.Logging;
using Ordering.API.Applications.Dtos.Requests;
using Ordering.API.Applications.Services.Interfaces;
using Ordering.API.Extensions;
using Ordering.Domain.Aggregates.OrderAggregate;
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
    IPublishEndpoint publishEndpoint) : IRequestHandler<CreateOrderCommand, Result>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly ILogger<CreateOrderCommandHandler> _logger = logger;
    private readonly IPaymentService _paymentService = paymentService; 
    private readonly IAccountServiceAPI _accountService = accountService; 
    private readonly ICatalogServiceAPI _catalogService = catalogService;
    private readonly IPublishEndpoint _publishEndpoint = publishEndpoint;

    public async Task<Result> Handle(CreateOrderCommand command, CancellationToken cancellationToken)
    {
        var requestItems = command.LineItems;
        await _unitOfWork.BeginTransactionAsync(IsolationLevel.ReadCommitted, cancellationToken); 
        try
        { 
            (var items, var isSuccess) = await _catalogService.SubstractStockAndGetDetailItems(requestItems.Select(x => (x.Id, x.Quantity)));

            if (!isSuccess) 
            {
                if (items is null)
                {
                    await RollbackAsync(cancellationToken);
                    return Result.Failure("An error has occured during check the item stock", ResponseStatus.InternalServerError);
                }
                 
                await RollbackAsync(cancellationToken);
                return Result.Failure("Can not proceed several items", ResponseStatus.BadRequest)
                    .WithData(items);
            } 

            var order = new Order(
                command.Currency,
                command.Address,
                command.Latitude,
                command.Longitude,
                command.BuyerId,
                "Eris", // get from account service
                command.BuyerType,
                command.IsOrderScheduled,
                command.ScheduledOn);  

            foreach (var lineItem in items?.Data ?? [])
            { 
                if (IsNullCatalogItems(lineItem.Name, lineItem.Sku, lineItem.Price, lineItem.Currency))
                {
                    await RollbackAndRecoveryStockAsync(requestItems, cancellationToken);
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
                        requestItems.First(x => x.Id == lineItem.Id).Quantity));
            }  

            foreach (var fleet in command.Fleets)
            {
                order.AddFleet(new(order.Id, fleet.Id, "Yamaha", "R1", "AG1L", "https://facebook.com/image.png"));
            }

            if (!string.IsNullOrWhiteSpace(command.CouponCode))
            {
                order.ApplyDiscount(Coupon.CreateValueDiscount(order.Id, command.CouponCode, 10, command.Currency, 10));
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

            await _unitOfWork.CommitTransactionAsync(cancellationToken); 

            return Result.Success(order.ToOrderResponseDto(), ResponseStatus.Created);
        }
        catch (AccountServiceHttpRequestException ex)
        { 
            LoggerHelper.LogError(_logger, ex);
            await _unitOfWork.RollbackTransactionAsync(cancellationToken);
            return Result.Failure(Messages.InternalServerError, ResponseStatus.InternalServerError);
        }
        catch (CatalogServiceHttpRequestException ex)
        {  
            LoggerHelper.LogError(_logger, ex);
            await _unitOfWork.RollbackTransactionAsync(cancellationToken);
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

        await _unitOfWork.CommitTransactionAsync();
    }

    private async Task RollbackAsync(CancellationToken cancellationToken)
    {
        await _unitOfWork.RollbackTransactionAsync(cancellationToken);
    }
}
