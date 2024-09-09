using Core.Events;
using Identity.API.Domain.Events;
using MassTransit;
using MediatR;

namespace Identity.API.Applications.IntegrationEvent.EventHandlers;

public class IdentityPhoneNumberConfirmedIntegrationEventHandler(IPublishEndpoint publishEndpoint) : INotificationHandler<PhoneNumberConfirmedDomainEvent>
{
    private readonly IPublishEndpoint _publishEndpoint = publishEndpoint;

    public async Task Handle(PhoneNumberConfirmedDomainEvent notification, CancellationToken cancellationToken)
    {
        var @event = new IdentityPhoneNumberConfirmedIntegrationEvent(notification.Sub, notification.PhoneNumber, notification.UserType);

        await _publishEndpoint.Publish(@event, cancellationToken);
    }
}