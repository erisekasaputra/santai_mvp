using Core.Events;
using Core.Events.Ordering;
using MediatR; 
using Ordering.Domain.Events;

namespace Ordering.API.Applications.DomainEventHandlers;

public class OrderRatedDomainEventHandler(IMediator mediator) : INotificationHandler<OrderRatedDomainEvent>
{
    private readonly IMediator _mediator = mediator;
    public async Task Handle(OrderRatedDomainEvent notification, CancellationToken cancellationToken)
    {
        await _mediator.Publish(new OrderRatedIntegrationEvent(
            notification.OrderId,
            notification.BuyerId,
            notification.Value,
            notification.Comment), cancellationToken);
    }
}
