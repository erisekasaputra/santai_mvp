using Identity.Contracts.IntegrationEvent;
using MassTransit;
using MediatR;

namespace Account.API.Applications.IntegrationEventHandlers;

public class StaffUserCreatedIntegrationEventHandler(
    IPublishEndpoint publishEndpoint) : INotificationHandler<StaffUserCreatedIntegrationEvent>
{
    private readonly IPublishEndpoint _publishEndpoint = publishEndpoint;
    public async Task Handle(StaffUserCreatedIntegrationEvent notification, CancellationToken cancellationToken)
    {
        await _publishEndpoint.Publish(notification, cancellationToken);
    }
}
