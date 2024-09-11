using Core.Events;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Ordering.Domain.Events;
using Ordering.Domain.SeedWork; 
//using Polly;
//using Polly.Retry;
using System.Data;

namespace Ordering.API.Applications.DomainEventHandlers;

public class OrderPaymentPaidDomainEventHandler : INotificationHandler<OrderPaymentPaidDomainEvent>
{ 
    private readonly IMediator _mediator; 

    public OrderPaymentPaidDomainEventHandler( 
        IMediator mediator)
    { 
        _mediator = mediator; 
    }

    public async Task Handle(OrderPaymentPaidDomainEvent notification, CancellationToken cancellationToken)
    {
        await _mediator.Publish(new OrderPaymentPaidIntegrationEvent(), cancellationToken); 
    }
}
