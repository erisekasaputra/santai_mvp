using Core.Events;
using MediatR; 
using Ordering.Domain.Events;  

namespace Ordering.API.Applications.DomainEventHandlers;

public class OrderRejectedDomainEventHandler : INotificationHandler<OrderRejectedByMechanicDomainEvent>
{ 
    private readonly IMediator _mediator; 

    public OrderRejectedDomainEventHandler( 
        IMediator mediator)
    { 
        _mediator = mediator; 
    }


    public async Task Handle(OrderRejectedByMechanicDomainEvent notification, CancellationToken cancellationToken)
    {
        var @event = new OrderRejectedByMechanicIntegrationEvent();
        await _mediator.Publish(@event, cancellationToken);
    }
}
