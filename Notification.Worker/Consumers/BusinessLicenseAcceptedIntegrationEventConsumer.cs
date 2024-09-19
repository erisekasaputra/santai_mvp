using Core.Events;
using MediatR;

namespace Notification.Worker.Consumers;

public class BusinessLicenseAcceptedIntegrationEventConsumer : INotificationHandler<BusinessLicenseAcceptedIntegrationEvent>
{
    public async Task Handle(BusinessLicenseAcceptedIntegrationEvent notification, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
