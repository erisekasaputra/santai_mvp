using Identity.Contracts.IntegrationEvent;
using MassTransit;
using MediatR;

namespace Account.API.Applications.IntegrationEventHandlers;

public class MechanicUserCreatedIntegrationEventHandler(
    IPublishEndpoint publishEndpoint) : INotificationHandler<MechanicUserCreatedIntegrationEvent>
{
    private readonly IPublishEndpoint _publishEndpoint = publishEndpoint;
    public async Task Handle(MechanicUserCreatedIntegrationEvent notification, CancellationToken cancellationToken)
    {
        await _publishEndpoint.Publish(notification, cancellationToken);
    }
}
