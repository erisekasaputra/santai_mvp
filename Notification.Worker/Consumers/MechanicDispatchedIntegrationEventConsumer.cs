using Core.Events;
using MassTransit;

namespace Notification.Worker.Consumers;

public class MechanicDispatchedIntegrationEventConsumer : IConsumer<MechanicDispatchedIntegrationEvent>
{
    public async Task Consume(ConsumeContext<MechanicDispatchedIntegrationEvent> context)
    {
        throw new NotImplementedException();
    }
}
