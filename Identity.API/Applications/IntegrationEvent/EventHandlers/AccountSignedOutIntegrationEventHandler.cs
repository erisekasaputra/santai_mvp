using Core.Events.Identity;
using MassTransit;
using MediatR;

namespace Identity.API.Applications.IntegrationEvent.EventHandlers;

public class AccountSignedOutIntegrationEventHandler(IPublishEndpoint publishEndpoint) : INotificationHandler<AccountSignedOutIntegrationEvent>
{
    private readonly IPublishEndpoint _publishEndpoint = publishEndpoint;
    public async Task Handle(AccountSignedOutIntegrationEvent notification, CancellationToken cancellationToken)
    {
        await _publishEndpoint.Publish(notification, cancellationToken);
    }
}
