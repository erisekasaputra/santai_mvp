using MassTransit.Transports;
using MassTransit;
using MediatR;
using Identity.Contracts.IntegrationEvent;

namespace Identity.API.IntegrationEvent.EventHandlers;

public class RegularUserDeletedIntegrationEventHandler(IPublishEndpoint publishEndpoint) : INotificationHandler<RegularUserDeletedIntegrationEvent>
{
    private readonly IPublishEndpoint _publishEndpoint = publishEndpoint;

    public async Task Handle(RegularUserDeletedIntegrationEvent notification, CancellationToken cancellationToken)
    {
        await _publishEndpoint.Publish(notification, cancellationToken);
    }
}
