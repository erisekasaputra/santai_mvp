using Core.Events;
using MassTransit;

namespace Notification.Worker.Consumers;

public class OrderRejectedByMechanicIntegrationEventConsumer : IConsumer<OrderRejectedByMechanicIntegrationEvent>
{
    public async Task Consume(ConsumeContext<OrderRejectedByMechanicIntegrationEvent> context)
    {
        throw new NotImplementedException();
    }
}
