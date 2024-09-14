using Core.Events;
using MediatR;

namespace Ordering.API.Applications.IntegrationEventHandlers;

public class OrderCancelledByBuyerIntegrationEventHandler : INotificationHandler<OrderCancelledByBuyerIntegrationEvent>
{
    public async Task Handle(OrderCancelledByBuyerIntegrationEvent notification, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
