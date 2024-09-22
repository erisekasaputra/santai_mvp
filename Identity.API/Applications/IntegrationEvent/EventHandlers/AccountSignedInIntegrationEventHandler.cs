using Core.Events.Identity;
using MassTransit;
using MediatR;

namespace Identity.API.Applications.IntegrationEvent.EventHandlers;

public class AccountSignedInIntegrationEventHandler(IPublishEndpoint publishEndpoint) : INotificationHandler<AccountSignedInIntegrationEvent>
{
    private readonly IPublishEndpoint _publishEndpoint = publishEndpoint;
    public async Task Handle(AccountSignedInIntegrationEvent notification, CancellationToken cancellationToken)
    {
        await _publishEndpoint.Publish(notification, cancellationToken);
    }
}
