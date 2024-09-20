
using Account.API.Applications.Services.Interfaces;
using Core.Events.Ordering;
using MassTransit;

namespace Account.API.Applications.Consumers;

public class OrderCancelledByMechanicIntegrationEventConsumer(
    IMechanicCache mechanicCache) : IConsumer<OrderCancelledByMechanicIntegrationEvent>
{
    private readonly IMechanicCache _mechanicCache = mechanicCache;
    public async Task Consume(ConsumeContext<OrderCancelledByMechanicIntegrationEvent> context)
    {
        (var isSuccess, var buyerId) = await _mechanicCache.CancelOrderByMechanic(
            context.Message.OrderId.ToString(),
            context.Message.MechanicId.ToString());

        if (isSuccess)
        {
            throw new Exception($"Failed to cancel the order by mechanic for order id {context.Message.OrderId}");
        } 
    }
}
