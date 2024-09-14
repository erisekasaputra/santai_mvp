using Core.Events; 
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
            notification.Order.Id, 
            notification.Order.Buyer.BuyerId, 
            notification.Order.Address.Latitude,
            notification.Order.Address.Longitude);

        await _mediator.Publish(@event, cancellationToken);
    }
}
