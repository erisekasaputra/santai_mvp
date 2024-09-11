using MediatR;
using Ordering.Domain.Events;

namespace Ordering.API.Applications.DomainEventHandlers;

public class OrderCreatedDomainEventHandler : INotificationHandler<OrderCreatedDomainEvent>
{
    public async Task Handle(OrderCreatedDomainEvent notification, CancellationToken cancellationToken)
    {
        await Task.Delay(1, cancellationToken); 
    }
}
