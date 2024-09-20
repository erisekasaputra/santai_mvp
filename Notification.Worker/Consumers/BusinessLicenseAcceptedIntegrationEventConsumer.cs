using Core.Events;
using MassTransit; 

namespace Notification.Worker.Consumers;

public class BusinessLicenseAcceptedIntegrationEventConsumer : IConsumer<BusinessLicenseAcceptedIntegrationEvent>
{
    public async Task Consume(ConsumeContext<BusinessLicenseAcceptedIntegrationEvent> context)
    {
        throw new NotImplementedException();
    } 
}
