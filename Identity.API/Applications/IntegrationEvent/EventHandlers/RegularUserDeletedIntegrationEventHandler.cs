using Core.Events.Account;
using MassTransit;
using MediatR;

namespace Identity.API.Applications.IntegrationEvent.EventHandlers;

public class RegularUserDeletedIntegrationEventHandler(IPublishEndpoint publishEndpoint) : INotificationHandler<RegularUserDeletedIntegrationEvent>
{
    private readonly IPublishEndpoint _publishEndpoint = publishEndpoint;

    public async Task Handle(RegularUserDeletedIntegrationEvent notification, CancellationToken cancellationToken)
    { 
        await _publishEndpoint.Publish(notification, cancellationToken);
    }
}
