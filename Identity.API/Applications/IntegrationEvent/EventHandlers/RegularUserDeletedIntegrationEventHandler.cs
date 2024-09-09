
using Core.Events;
using Identity.API.Domain.Events;
using MassTransit;
using MediatR;

namespace Identity.API.Applications.IntegrationEvent.EventHandlers;

public class RegularUserDeletedIntegrationEventHandler(IPublishEndpoint publishEndpoint) : INotificationHandler<RegularUserDeleteDomainEvent>
{
    private readonly IPublishEndpoint _publishEndpoint = publishEndpoint;

    public async Task Handle(RegularUserDeleteDomainEvent notification, CancellationToken cancellationToken)
    {
        var @event = new RegularUserDeletedIntegrationEvent(notification.UserId);

        await _publishEndpoint.Publish(@event, cancellationToken);
    }
}
