using Core.Events;
using MassTransit;

namespace Notification.Worker.Consumers;

public class OtpRequestedIntegrationEventConsumer : IConsumer<OtpRequestedIntegrationEvent>
{
    public async Task Consume(ConsumeContext<OtpRequestedIntegrationEvent> context)
    {
        throw new NotImplementedException();
    }
}
