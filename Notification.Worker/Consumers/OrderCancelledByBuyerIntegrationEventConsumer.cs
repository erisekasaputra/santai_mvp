using Core.Events;
using MassTransit;

namespace Notification.Worker.Consumers;

public class OrderCancelledByBuyerIntegrationEventConsumer : IConsumer<OrderCancelledByBuyerIntegrationEvent>
{
    public async Task Consume(ConsumeContext<OrderCancelledByBuyerIntegrationEvent> context)
    {
        throw new NotImplementedException();
    }
}
