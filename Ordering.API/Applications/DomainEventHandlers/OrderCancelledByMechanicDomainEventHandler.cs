using Core.Events;
using Core.Events.Ordering;
using MediatR; 
using Ordering.Domain.Events;  

namespace Ordering.API.Applications.DomainEventHandlers;

public class OrderCancelledByMechanicDomainEventHandler : INotificationHandler<OrderCancelledByMechanicDomainEvent>
{ 
    private readonly IMediator _mediator;  
    public OrderCancelledByMechanicDomainEventHandler( 
        IMediator mediator)
    { 
        _mediator = mediator; 
    }

    public async Task Handle(OrderCancelledByMechanicDomainEvent notification, CancellationToken cancellationToken)
    { 
        await _mediator.Publish(
            new OrderCancelledByMechanicIntegrationEvent(
                notification.OrderId,
                notification.BuyerId,
                notification.BuyerName,
                notification.MechanicId,
                notification.MechanicName), cancellationToken);
    }
}
