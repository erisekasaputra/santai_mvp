using MediatR;
using Order.API.SeedWorks;
using Order.Domain.Aggregates.OrderAggregate;
using Order.Domain.Enumerations;

namespace Order.API.Applications.Commands.Orders.CreateOrder;

public class CreateOrderCommandHandler : IRequestHandler<CreateOrderCommand, Result>
{
    private readonly ILogger<CreateOrderCommandHandler> _logger;    
    public CreateOrderCommandHandler(ILogger<CreateOrderCommandHandler> logger)
    {
        _logger = logger;
    }

    public async Task<Result> Handle(CreateOrderCommand command, CancellationToken cancellationToken)
    {
        try
        {
            var order = new Ordering(Currency.MYR, "", 1, 1, Guid.NewGuid(), "", UserType.RegularUser, true, DateTime.UtcNow);

            order.AddOrderItem(Guid.NewGuid(), "", "", 1, Currency.MYR, 1);
            order.AddOrderItem(Guid.NewGuid(), "", "", 1, Currency.MYR, 1);  

            order.CalculateOrderAmount(); // the most end of line 

            order.AddFeeByPercentage(FeeDescription.MechanicFee, 100);
            order.AddFeeByValue(FeeDescription.ServiceFee, 50, Currency.MYR);

            order.CalculateGrandTotal();

            return Result.Success(order);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.InnerException?.Message);
            return Result.Failure(Messages.InternalServerError, ResponseStatus.InternalServerError);
        }
    }
}
