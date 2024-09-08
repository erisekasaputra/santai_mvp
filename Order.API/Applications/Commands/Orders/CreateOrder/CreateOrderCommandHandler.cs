using Core.Messages;
using Core.Results;
using MediatR; 
using Order.API.Applications.Services.Interfaces;  
using Order.Domain.Aggregates.OrderAggregate;
using Order.Domain.Enumerations;
using Order.Domain.Exceptions;
using Order.Domain.ValueObjects;

namespace Order.API.Applications.Commands.Orders.CreateOrder;

public class CreateOrderCommandHandler(ILogger<CreateOrderCommandHandler> logger, IPaymentService paymentService) : IRequestHandler<CreateOrderCommand, Result>
{
    private readonly ILogger<CreateOrderCommandHandler> _logger = logger;
    private readonly IPaymentService _paymentService = paymentService;
    private const Currency GlobalCurrency = Currency.MYR;

    public async Task<Result> Handle(CreateOrderCommand command, CancellationToken cancellationToken) 
    { 
        await Task.Delay(1, cancellationToken);
        try
        { 
            var order = new Ordering(
                GlobalCurrency,
                command.Address,
                command.Latitude,
                command.Longitude,
                command.BuyerId,
                "Buyer Name",
                command.UserType,
                command.IsOrderScheduled,
                command.ScheduledOn);

            foreach (var lineItem in command.LineItems) 
            {
                order.AddOrderItem(lineItem.Id, "Sampo 1", "SKU123", 10, GlobalCurrency, lineItem.Quantity);
            }   
             
            foreach (var fleet in command.Fleets)
            {
                order.AddFleet(fleet.Id, "", "", "", ""); 
            } 

            order.ApplyDiscount(Coupon.CreateValueDiscount(order.Id, command.CouponCode, 10, GlobalCurrency, 10));
            order.ApplyTax(new Tax(10, GlobalCurrency));
            order.ApplyFee(Fee.CreateByValue(order.Id, FeeDescription.MechanicFee, 10, GlobalCurrency)); 
            order.ApplyFee(Fee.CreateByPercentage(order.Id, FeeDescription.ServiceFee, 10, GlobalCurrency));  

            order.CalculateGrandTotal();  
            

            return Result.Success(order);
        }
        catch(ArgumentNullException ex)
        { 
            return Result.Failure(ex.Message, ResponseStatus.BadRequest);
        }
        catch(DomainException ex)
        { 
            return Result.Failure(ex.Message, ResponseStatus.BadRequest); 
        }
        catch(NotImplementedException ex)
        {
            _logger.LogError(ex, "An error occured , Error: {error}, Detail: {Detail}", ex.Message, ex.InnerException?.Message);
            return Result.Failure(Messages.InternalServerError, ResponseStatus.InternalServerError);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occured , Error: {error}, Detail: {Detail}", ex.Message, ex.InnerException?.Message);
            return Result.Failure(Messages.InternalServerError, ResponseStatus.InternalServerError);
        }
    }
}
