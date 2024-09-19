using Core.Events;
using MassTransit;

namespace Notification.Worker.Consumers;

public class OrderCreatedIntegrationEventConsumer : IConsumer<OrderCreatedIntegrationEvent>
{
    public async Task Consume(ConsumeContext<OrderCreatedIntegrationEvent> context)
    {
        throw new NotImplementedException();
    }
}
