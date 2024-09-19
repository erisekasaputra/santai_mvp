using Core.Events;
using MediatR;
using Ordering.Domain.Events;

namespace Ordering.API.Applications.DomainEventHandlers;

public class RefundPaidDomainEventHandler(IMediator mediator) : INotificationHandler<RefundPaidDomainEvent>
{
    private readonly IMediator _mediator = mediator;
    public async Task Handle(RefundPaidDomainEvent notification, CancellationToken cancellationToken)
    {
        await _mediator.Send(new RefundPaidIntegrationEvent(
            notification.OrderId,
            notification.BuyerId,
            notification.Amount,
            notification.Currency));
    }
}
