using Account.Domain.Events;
using Core.Events.Account;
using MediatR;

namespace Account.API.Applications.DomainEventHandlers;

public class AccountMechanicAutoSelectedToAnOrderDomainEventHandler(
    IMediator mediator) : INotificationHandler<AccountMechanicAutoSelectedToAnOrderDomainEvent>
{
    private readonly IMediator _mediator = mediator;
    public async Task Handle(AccountMechanicAutoSelectedToAnOrderDomainEvent notification, CancellationToken cancellationToken)
    {
        var @event = new MechanicAutoSelectedIntegrationEvent(notification.OrderId, notification.BuyerId, notification.MechanicId);

        await _mediator.Publish(@event, cancellationToken);
    }
}
