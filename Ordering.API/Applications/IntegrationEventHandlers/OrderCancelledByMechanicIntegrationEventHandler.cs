using Core.Events;
using MediatR;

namespace Ordering.API.Applications.IntegrationEventHandlers;

public class OrderCancelledByMechanicIntegrationEventHandler : INotificationHandler<OrderCancelledByMechanicIntegrationEvent>
{
    public async Task Handle(OrderCancelledByMechanicIntegrationEvent notification, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
