
using Account.API.Applications.Services.Interfaces;
using Core.Events; 
using MassTransit; 

namespace Account.API.Applications.Consumers;

public class OrderCancelledByMechanicIntegrationEventConsumer(
    IMechanicCache mechanicCache) : IConsumer<OrderCancelledByMechanicIntegrationEvent>
{
    private readonly IMechanicCache _mechanicCache = mechanicCache;
    public async Task Consume(ConsumeContext<OrderCancelledByMechanicIntegrationEvent> context)
    {
        var result = await _mechanicCache.CancelOrderByMechanic(
            context.Message.MechanicId.ToString(),
            context.Message.OrderId.ToString());

        if (result)
        {
            throw new Exception($"Failed to cancel the order by mechanic for order id {context.Message.OrderId}");
        } 
    }
}
