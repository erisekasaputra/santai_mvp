using Core.Events.Identity;
using Identity.API.Domain.Events;
using MediatR;

namespace Identity.API.Applications.DomainEventHandlers;

public class AccountSignedInDomainEventHandler(IMediator mediator) : INotificationHandler<AccountSignedInDomainEvent>
{
    private readonly IMediator _mediator = mediator;
    public async Task Handle(AccountSignedInDomainEvent notification, CancellationToken cancellationToken)
    {
        await _mediator.Publish(new 
            AccountSignedInIntegrationEvent(
                notification.UserId, 
                notification.DeviceId, 
                notification.PhoneNumber,
                notification.Email), cancellationToken);
    }
}
