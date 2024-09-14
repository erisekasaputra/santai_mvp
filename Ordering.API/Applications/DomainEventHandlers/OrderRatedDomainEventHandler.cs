using Core.Events;
using MediatR; 
using Ordering.Domain.Events;

namespace Ordering.API.Applications.DomainEventHandlers;

public class OrderRatedDomainEventHandler(IMediator mediator) : INotificationHandler<OrderRatedDomainEvent>
{
    private readonly IMediator _mediator = mediator;
    public async Task Handle(OrderRatedDomainEvent notification, CancellationToken cancellationToken)
    {
        await _mediator.Send(new OrderRatedIntegrationEvent(
            notification.Order.Id,
            notification.Order.Buyer.BuyerId,
            notification.Order.Rating!.Value,
            notification.Order.Rating!.Comment), cancellationToken);
    }
}
