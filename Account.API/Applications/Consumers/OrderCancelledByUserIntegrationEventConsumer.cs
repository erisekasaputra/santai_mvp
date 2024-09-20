using Account.API.Applications.Services.Interfaces;
using Core.Events.Ordering;
using MassTransit;

namespace Account.API.Applications.Consumers;

public class OrderCancelledByUserIntegrationEventConsumer(
    IMechanicCache mechanicCache) : IConsumer<OrderCancelledByBuyerIntegrationEvent>
{
    private readonly IMechanicCache _mechanicCache = mechanicCache; 
    public async Task Consume(ConsumeContext<OrderCancelledByBuyerIntegrationEvent> context)
    {
        (var isSuccess, var mechanicId) = await _mechanicCache.CancelOrderByUser(
            context.Message.OrderId.ToString(),
            context.Message.BuyerId.ToString());

        if (isSuccess)
        {
            throw new Exception($"Failed when cancelling the order {context.Message.OrderId} by buyer");
        }
    }
}
