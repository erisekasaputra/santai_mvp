using Core.Events;
using Core.Events.Ordering;
using MediatR; 
using Ordering.Domain.Events; 

namespace Ordering.API.Applications.DomainEventHandlers;

public class OrderFindingMechanicDomainEventHandler : INotificationHandler<OrderFindingMechanicDomainEvent>
{ 
    private readonly IMediator _mediator;  
    public OrderFindingMechanicDomainEventHandler( 
        IMediator mediator)
    { 
        _mediator = mediator; 
    }
    public async Task Handle(OrderFindingMechanicDomainEvent notification, CancellationToken cancellationToken)
    {
        var @event = new OrderFindingMechanicIntegrationEvent(
            notification.OrderId, 
            notification.BuyerId, 
            notification.Latitude,
            notification.Longitude);

        await _mediator.Publish(@event, cancellationToken);
    }
}
