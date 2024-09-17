using Core.Events;
using MediatR; 
using Ordering.Domain.Events;  

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
        await _mediator.Publish(new OrderPaymentPaidIntegrationEvent(
            notification.OrderId,
            notification.BuyerId,
            notification.Amount,
            notification.Currency), cancellationToken); 
    }
}
