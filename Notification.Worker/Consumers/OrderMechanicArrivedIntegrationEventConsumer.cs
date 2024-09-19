using Core.Events;
using MassTransit;

namespace Notification.Worker.Consumers;

public class OrderMechanicArrivedIntegrationEventConsumer : IConsumer<OrderMechanicArrivedIntegrationEvent>
{
    public async Task Consume(ConsumeContext<OrderMechanicArrivedIntegrationEvent> context)
    {
        throw new NotImplementedException();
    }
}
