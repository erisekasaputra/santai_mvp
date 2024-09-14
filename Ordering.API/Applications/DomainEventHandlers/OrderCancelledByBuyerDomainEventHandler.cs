using Core.Events;
using MediatR; 
using Ordering.Domain.Events;

namespace Ordering.API.Applications.DomainEventHandlers;

public class OrderCancelledByBuyerDomainEventHandler(IMediator mediator) : INotificationHandler<OrderCancelledByBuyerDomainEvent>
{
    private readonly IMediator _mediator = mediator;
    public async Task Handle(OrderCancelledByBuyerDomainEvent notification, CancellationToken cancellationToken)
    {
        await _mediator.Send(new OrderCancelledByBuyerIntegrationEvent(
            notification.Order.Id,
            notification.Order.Buyer.BuyerId,
            notification.Order.Mechanic?.MechanicId), cancellationToken);
    }
}
