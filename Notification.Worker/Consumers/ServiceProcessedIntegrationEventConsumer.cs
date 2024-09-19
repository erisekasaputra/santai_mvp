using Core.Events;
using MassTransit;

namespace Notification.Worker.Consumers;

public class ServiceProcessedIntegrationEventConsumer : IConsumer<ServiceProcessedIntegrationEvent>
{
    public async Task Consume(ConsumeContext<ServiceProcessedIntegrationEvent> context)
    {
        throw new NotImplementedException();
    }
}
