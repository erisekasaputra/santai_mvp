using Core.Events;
using MassTransit;

namespace Notification.Worker.Consumers;

public class OrderFindingMechanicIntegrationEventConsumer : IConsumer<OrderFindingMechanicIntegrationEvent>
{
    public async Task Consume(ConsumeContext<OrderFindingMechanicIntegrationEvent> context)
    {
        throw new NotImplementedException();
    }
}
