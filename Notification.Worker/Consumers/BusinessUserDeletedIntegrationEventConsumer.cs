using Core.Events;
using MassTransit;

namespace Notification.Worker.Consumers;

public class BusinessUserDeletedIntegrationEventConsumer : IConsumer<BusinessUserDeletedIntegrationEvent>
{
    public async Task Consume(ConsumeContext<BusinessUserDeletedIntegrationEvent> context)
    {
        throw new NotImplementedException();
    }
}
