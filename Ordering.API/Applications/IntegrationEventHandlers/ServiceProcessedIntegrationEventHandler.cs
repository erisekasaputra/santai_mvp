using Core.Events;
using MediatR;

namespace Ordering.API.Applications.IntegrationEventHandlers;

public class ServiceProcessedIntegrationEventHandler : INotificationHandler<ServiceProcessedIntegrationEvent>
{
    public async Task Handle(ServiceProcessedIntegrationEvent notification, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
