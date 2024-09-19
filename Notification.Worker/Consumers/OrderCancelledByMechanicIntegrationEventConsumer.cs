using Core.Events;
using MassTransit;

namespace Notification.Worker.Consumers;

public class OrderCancelledByMechanicIntegrationEventConsumer : IConsumer<OrderCancelledByMechanicIntegrationEvent>
{
    public async Task Consume(ConsumeContext<OrderCancelledByMechanicIntegrationEvent> context)
    {
        throw new NotImplementedException();
    }
}
