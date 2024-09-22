using Core.Events.Identity;
using Identity.API.Domain.Events;
using MediatR;

namespace Identity.API.Applications.DomainEventHandlers;

public class AccountSignedOutDomainEventHandler(IMediator mediator) : INotificationHandler<AccountSignedOutDomainEvent>
{
    private readonly IMediator _mediator = mediator;
    public async Task Handle(AccountSignedOutDomainEvent notification, CancellationToken cancellationToken)
    {
        await _mediator.Publish(new AccountSignedOutIntegrationEvent(notification.UserId, notification.DeviceId), cancellationToken);
    }
}
