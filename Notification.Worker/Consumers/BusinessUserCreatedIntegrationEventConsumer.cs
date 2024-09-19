using Core.Events;
using MassTransit;

namespace Notification.Worker.Consumers;

public class BusinessUserCreatedIntegrationEventConsumer : IConsumer<BusinessUserCreatedIntegrationEvent>
{
    public async Task Consume(ConsumeContext<BusinessUserCreatedIntegrationEvent> context)
    {
        throw new NotImplementedException();
    }
}
