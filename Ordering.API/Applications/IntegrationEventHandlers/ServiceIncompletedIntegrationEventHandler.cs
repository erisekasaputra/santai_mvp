using Core.Events;
using MediatR;

namespace Ordering.API.Applications.IntegrationEventHandlers;

public class ServiceIncompletedIntegrationEventHandler : INotificationHandler<ServiceIncompletedIntegrationEvent>
{
    public async Task Handle(ServiceIncompletedIntegrationEvent notification, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
