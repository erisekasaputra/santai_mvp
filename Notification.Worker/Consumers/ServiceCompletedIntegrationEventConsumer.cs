using Core.Events;
using MassTransit;

namespace Notification.Worker.Consumers;

public class ServiceCompletedIntegrationEventConsumer : IConsumer<ServiceCompletedIntegrationEvent>
{
    public async Task Consume(ConsumeContext<ServiceCompletedIntegrationEvent> context)
    {
        throw new NotImplementedException();
    }
}
