using Core.Events;
using Identity.API.Domain.Events;
using MassTransit;
using MediatR;

namespace Identity.API.Applications.IntegrationEvent.EventHandlers;

public class IdentityEmailAssignedToAUserIntegrationEventHandler(IPublishEndpoint publishEndpoint) : INotificationHandler<EmailAssignedToAUserDomainEvent>
{
    private readonly IPublishEndpoint _publishEndpoint = publishEndpoint;
    public async Task Handle(EmailAssignedToAUserDomainEvent notification, CancellationToken cancellationToken)
    {
        var @event = new IdentityEmailAssignedToAUserIntegrationEvent(
            notification.Sub, notification.Email, notification.UserType);

        await _publishEndpoint.Publish(@event, cancellationToken);
    }
}
