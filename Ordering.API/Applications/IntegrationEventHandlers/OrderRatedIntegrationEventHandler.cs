using Core.Events;
using MediatR;

namespace Ordering.API.Applications.IntegrationEventHandlers;

public class OrderRatedIntegrationEventHandler : INotificationHandler<OrderRatedIntegrationEvent>
{
    public Task Handle(OrderRatedIntegrationEvent notification, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
