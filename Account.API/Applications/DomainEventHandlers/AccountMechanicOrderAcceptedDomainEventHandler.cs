using Account.Domain.Events;
using Core.Events;
using MediatR;

namespace Account.API.Applications.DomainEventHandlers;

public class AccountMechanicOrderAcceptedDomainEventHandler(
    IMediator mediator) : INotificationHandler<AccountMechanicOrderAcceptedDomainEvent>
{
    private readonly IMediator _mediator = mediator;
    public async Task Handle(AccountMechanicOrderAcceptedDomainEvent notification, CancellationToken cancellationToken)
    {
        await _mediator.Publish(
            new AccountMechanicOrderAcceptedIntegrationEvent(notification.OrderId, notification.MechanicId),
            cancellationToken);
    }
}
