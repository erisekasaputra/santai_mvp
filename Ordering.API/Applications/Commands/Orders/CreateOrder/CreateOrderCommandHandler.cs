using Core.Exceptions;
using Core.Messages;
using Core.Results;
using MediatR; 
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
    ICatalogServiceAPI catalogService) : IRequestHandler<CreateOrderCommand, Result>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly ILogger<CreateOrderCommandHandler> _logger = logger;
    private readonly IPaymentService _paymentService = paymentService; 
    private readonly IAccountServiceAPI _accountService = accountService; 
    private readonly ICatalogServiceAPI _catalogService = catalogService; 

    public async Task<Result> Handle(CreateOrderCommand command, CancellationToken cancellationToken)
    {
        await _unitOfWork.BeginTransactionAsync(IsolationLevel.ReadCommitted, cancellationToken);
        Guid orderId;
        try
        {
            (var items, var isSuccess) = await _catalogService.SubstractStockAndGetDetailItems(command.LineItems.Select(x => (x.Id, x.Quantity)));

            if (!isSuccess) 
            {
                if (items is null)
                {
                    return Result.Failure("An error has occured during check the item stock", ResponseStatus.InternalServerError);
                }

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

            orderId = order.Id;

            foreach (var lineItem in command.LineItems)
            {
                order.AddOrderItem(new(orderId, lineItem.Id, "Sampo 1", "SKU123", 10, command.Currency, lineItem.Quantity));
            }

            foreach (var fleet in command.Fleets)
            {
                order.AddFleet(new(orderId, fleet.Id, "Yamaha", "R1", "AG1L", "https://facebook.com/image.png"));
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
        catch (ArgumentNullException ex)
        {
            await _unitOfWork.RollbackTransactionAsync(cancellationToken);
            return Result.Failure(ex.Message, ResponseStatus.BadRequest);
        }
        catch (DomainException ex)
        {
            await _unitOfWork.RollbackTransactionAsync(cancellationToken);
            return Result.Failure(ex.Message, ResponseStatus.BadRequest);
        }
        catch (NotImplementedException ex)
        {
            await _unitOfWork.RollbackTransactionAsync(cancellationToken);
            _logger.LogError(ex, "An error occured , Error: {error}, Detail: {Detail}", ex.Message, ex.InnerException?.Message);
            return Result.Failure(Messages.InternalServerError, ResponseStatus.InternalServerError);
        }
        catch (Exception ex)
        {
            await _unitOfWork.RollbackTransactionAsync(cancellationToken);
            _logger.LogError(ex, "An error occured , Error: {error}, Detail: {Detail}", ex.Message, ex.InnerException?.Message);
            return Result.Failure(Messages.InternalServerError, ResponseStatus.InternalServerError);
        }
    }
}
