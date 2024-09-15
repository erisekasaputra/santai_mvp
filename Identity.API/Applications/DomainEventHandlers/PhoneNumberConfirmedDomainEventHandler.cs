using Core.Events;
using Identity.API.Domain.Events; 
using MediatR;

namespace Identity.API.Applications.DomainEventHandlers;

public class PhoneNumberConfirmedDomainEventHandler(IMediator mediator) : INotificationHandler<PhoneNumberConfirmedDomainEvent>
{
    private readonly IMediator _mediator = mediator;

    public async Task Handle(PhoneNumberConfirmedDomainEvent notification, CancellationToken cancellationToken)
    {
        var @event = new IdentityPhoneNumberConfirmedIntegrationEvent(notification.Sub, notification.PhoneNumber, notification.UserType);

        await _mediator.Publish(@event, cancellationToken);
    }
}