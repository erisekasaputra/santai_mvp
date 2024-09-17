using Account.API.Applications.Services.Interfaces;
using Core.Events; 
using MassTransit;

namespace Account.API.Applications.Consumers;

public class OrderCancelledByUserIntegrationEventConsumer(
    IMechanicCache mechanicCache) : IConsumer<OrderCancelledByUserIntegrationEvent>
{
    private readonly IMechanicCache _mechanicCache = mechanicCache; 
    public async Task Consume(ConsumeContext<OrderCancelledByUserIntegrationEvent> context)
    {
        var result = await _mechanicCache.CancelOrderByUser(
            context.Message.UserId.ToString(),
            context.Message.OrderId.ToString());

        if (result)
        {
            throw new Exception($"Failed when cancelling the order {context.Message.OrderId} by buyer");
        }
    }
}
