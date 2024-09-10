using Core.Enumerations;
using Core.Exceptions;
using Core.Messages;
using Core.Results;
using MediatR; 
using Order.API.Applications.Services.Interfaces; 
using Order.Domain.Aggregates.OrderAggregate;
using Order.Domain.Enumerations; 
using Order.Domain.SeedWork; 
using System.Data; 

namespace Order.API.Applications.Commands.Orders.CreateOrder;

public class CreateOrderCommandHandler(
    ILogger<CreateOrderCommandHandler> logger,
    IPaymentService paymentService,
    IUnitOfWork unitOfWork) : IRequestHandler<CreateOrderCommand, Result>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly ILogger<CreateOrderCommandHandler> _logger = logger;
    private readonly IPaymentService _paymentService = paymentService;
    private const Currency GlobalCurrency = Currency.MYR;
    public async Task<Result> Handle(CreateOrderCommand command, CancellationToken cancellationToken) 
    { 
        await _unitOfWork.BeginTransactionAsync(IsolationLevel.ReadCommitted, cancellationToken); 
        Guid orderId; 
        try
        { 
            var order = new Ordering(
                GlobalCurrency,
                command.Address,
                command.Latitude,
                command.Longitude,
                command.BuyerId,
                "Eris Eka Saputra",
                command.UserType,
                command.IsOrderScheduled,
                command.ScheduledOn);

            orderId = order.Id;

            foreach (var lineItem in command.LineItems) 
            {
                order.AddOrderItem(new (orderId, lineItem.Id, "Sampo 1", "SKU123", 10, GlobalCurrency, lineItem.Quantity));
            }   
             
            foreach (var fleet in command.Fleets)
            {
                order.AddFleet(new (orderId, fleet.Id, "Yamaha", "R1", "AG1L", "https://facebook.com/image.png")); 
            } 

            if (!string.IsNullOrWhiteSpace(command.CouponCode))
            {
                order.ApplyDiscount(Coupon.CreateValueDiscount(order.Id, command.CouponCode, 10, GlobalCurrency, 10)); 
            }
            //order.ApplyTax(new Tax(10, GlobalCurrency));
            order.ApplyFee(Fee.CreateByValue(order.Id, FeeDescription.MechanicFee, 10, GlobalCurrency)); 
            order.ApplyFee(Fee.CreateByValue(order.Id, FeeDescription.ServiceFee, 10, GlobalCurrency));  

            order.CalculateGrandTotal();

            var payment = new Payment(order.Id, order.GrandTotal.Amount, order.Currency, DateTime.UtcNow, "", ""); 

            await _unitOfWork.Orders.CreateAsync(order, cancellationToken); 
            
            await _unitOfWork.CommitTransactionAsync(cancellationToken);  
             
            return Result.Success("Order successfully created", ResponseStatus.Ok);
        }
        catch(ArgumentNullException ex)
        { 
            await _unitOfWork.RollbackTransactionAsync(cancellationToken);
            return Result.Failure(ex.Message, ResponseStatus.BadRequest);
        }
        catch(DomainException ex)
        {
            await _unitOfWork.RollbackTransactionAsync(cancellationToken);
            return Result.Failure(ex.Message, ResponseStatus.BadRequest); 
        }
        catch(NotImplementedException ex)
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
