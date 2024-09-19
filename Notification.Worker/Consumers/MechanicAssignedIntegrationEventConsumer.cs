using Core.Events;
using MassTransit;

namespace Notification.Worker.Consumers;

public class MechanicAssignedIntegrationEventConsumer : IConsumer<MechanicAssignedIntegrationEvent>
{
    public async Task Consume(ConsumeContext<MechanicAssignedIntegrationEvent> context)
    {
        throw new NotImplementedException();
    }
}
