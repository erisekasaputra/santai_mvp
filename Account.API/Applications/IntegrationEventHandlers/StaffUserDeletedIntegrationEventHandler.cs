using Core.Events;  
using MassTransit;
using MediatR;

namespace Account.API.Applications.IntegrationEventHandlers;

public class StaffUserDeletedIntegrationEventHandler(
    IPublishEndpoint publishEndpoint) : INotificationHandler<StaffUserDeletedIntegrationEvent>
{
    private readonly IPublishEndpoint _publishEndpoint = publishEndpoint;
    public async Task Handle(StaffUserDeletedIntegrationEvent notification, CancellationToken cancellationToken)
    {
        await _publishEndpoint.Publish(notification, cancellationToken);
    }
}
