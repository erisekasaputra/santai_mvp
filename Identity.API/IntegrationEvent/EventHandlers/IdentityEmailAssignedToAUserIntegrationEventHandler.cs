using Identity.API.Domain.Events;
using Identity.Contracts;
using MassTransit;
using MediatR;

namespace Identity.API.IntegrationEvent.EventHandlers;

public class IdentityEmailAssignedToAUserIntegrationEventHandler(IPublishEndpoint publishEndpoint) : INotificationHandler<EmailAssignedToAUserDomainEvent>
{
    private readonly IPublishEndpoint _publishEndpoint = publishEndpoint;

    public async Task Handle(EmailAssignedToAUserDomainEvent notification, CancellationToken cancellationToken)
    {
        var @event = new IdentityEmailAssignedToAUserDomainEvent(notification.Sub, notification.Email);

        await _publishEndpoint.Publish(@event, cancellationToken);
    }
}
