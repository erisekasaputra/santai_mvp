using Core.Events;
using MediatR;

namespace Ordering.API.Applications.IntegrationEventHandlers;

public class OrderFindingMechanicIntegrationEventHandler : INotificationHandler<OrderFindingMechanicIntegrationEvent>
{
    public async Task Handle(OrderFindingMechanicIntegrationEvent notification, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
