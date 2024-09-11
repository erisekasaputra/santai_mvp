using Core.Events;
using MediatR; 
using Ordering.Domain.Events;
using Ordering.Domain.SeedWork;


//using Polly;
//using Polly.Retry;
using System.Data;

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
        await _mediator.Publish(new OrderCancelledByMechanicIntegrationEvent(), cancellationToken);
    }
}
