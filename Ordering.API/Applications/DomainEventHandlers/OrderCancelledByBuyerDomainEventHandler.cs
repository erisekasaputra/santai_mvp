using Core.Events;
using Core.Events.Ordering;
using MediatR; 
using Ordering.Domain.Events;

namespace Ordering.API.Applications.DomainEventHandlers;

public class OrderCancelledByBuyerDomainEventHandler(IMediator mediator) : INotificationHandler<OrderCancelledByBuyerDomainEvent>
{
    private readonly IMediator _mediator = mediator;
    public async Task Handle(OrderCancelledByBuyerDomainEvent notification, CancellationToken cancellationToken)
    {
        await _mediator.Publish(new OrderCancelledByBuyerIntegrationEvent(
            notification.OrderId,
            notification.BuyerId, 
            notification.BuyerName,
            notification.MechanicId,
            notification.MechanicName), cancellationToken);
    }
}
