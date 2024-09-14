using Core.Events;
using MediatR;

namespace Ordering.API.Applications.IntegrationEventHandlers;

public class MechanicDispatchedIntegrationEventHandler : INotificationHandler<MechanicDispatchedIntegrationEvent>
{
    public async Task Handle(MechanicDispatchedIntegrationEvent notification, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
