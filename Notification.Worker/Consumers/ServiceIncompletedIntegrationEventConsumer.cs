using Core.Events;
using MassTransit;

namespace Notification.Worker.Consumers;

public class ServiceIncompletedIntegrationEventConsumer : IConsumer<ServiceIncompletedIntegrationEvent>
{
    public async Task Consume(ConsumeContext<ServiceIncompletedIntegrationEvent> context)
    {
        throw new NotImplementedException();
    }
}
