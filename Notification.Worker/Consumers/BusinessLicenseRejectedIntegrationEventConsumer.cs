using Core.Events;
using MassTransit;

namespace Notification.Worker.Consumers;

public class BusinessLicenseRejectedIntegrationEventConsumer : IConsumer<BusinessLicenseRejectedIntegrationEvent>
{
    public async Task Consume(ConsumeContext<BusinessLicenseRejectedIntegrationEvent> context)
    {
        throw new NotImplementedException();
    }
}
