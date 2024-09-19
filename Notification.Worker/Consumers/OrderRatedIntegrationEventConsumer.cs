using Core.Events;
using MassTransit;

namespace Notification.Worker.Consumers;

public class OrderRatedIntegrationEventConsumer : IConsumer<OrderRatedIntegrationEvent>
{
    public async Task Consume(ConsumeContext<OrderRatedIntegrationEvent> context)
    {
        throw new NotImplementedException();
    }
}
