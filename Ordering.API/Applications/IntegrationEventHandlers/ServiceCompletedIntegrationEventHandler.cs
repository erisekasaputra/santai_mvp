using Core.Events;
using MediatR;

namespace Ordering.API.Applications.IntegrationEventHandlers;

public class ServiceCompletedIntegrationEventHandler : INotificationHandler<ServiceCompletedIntegrationEvent>
{
    public async Task Handle(ServiceCompletedIntegrationEvent notification, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
