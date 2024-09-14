using Core.Events;
using MediatR;

namespace Ordering.API.Applications.IntegrationEventHandlers;

public class MechanicArrivedIntegrationEventHandler : INotificationHandler<MechanicAssignedIntegrationEvent>
{
    public async Task Handle(MechanicAssignedIntegrationEvent notification, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
