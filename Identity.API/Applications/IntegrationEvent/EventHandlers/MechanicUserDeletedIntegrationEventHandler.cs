
using Core.Events;
using Identity.API.Domain.Events;
using MassTransit;
using MediatR;

namespace Identity.API.Applications.IntegrationEvent.EventHandlers;

public class MechanicUserDeletedIntegrationEventHandler(IPublishEndpoint publishEndpoint) : INotificationHandler<MechanicUserDeletedDomainEvent>
{
    private readonly IPublishEndpoint _publishEndpoint = publishEndpoint;
    public async Task Handle(MechanicUserDeletedDomainEvent notification, CancellationToken cancellationToken)
    {
        var @event = new MechanicUserDeletedIntegrationEvent(notification.UserId);

        await _publishEndpoint.Publish(@event, cancellationToken);
    }
}
