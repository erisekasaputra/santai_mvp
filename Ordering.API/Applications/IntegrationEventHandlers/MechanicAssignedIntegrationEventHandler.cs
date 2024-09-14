using Core.Events;
using MediatR;

namespace Ordering.API.Applications.IntegrationEventHandlers;

public class MechanicAssignedIntegrationEventHandler : INotificationHandler<MechanicAssignedIntegrationEvent>
{
    public Task Handle(MechanicAssignedIntegrationEvent notification, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
