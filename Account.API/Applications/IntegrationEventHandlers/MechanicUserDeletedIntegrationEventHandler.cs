using Core.Events; 
using MassTransit;
using MediatR;

namespace Account.API.Applications.IntegrationEventHandlers;

public class MechanicUserDeletedIntegrationEventHandler(
    IPublishEndpoint publishEndpoint) : INotificationHandler<MechanicUserDeletedIntegrationEvent>
{
    private readonly IPublishEndpoint _publishEndpoint = publishEndpoint;
    public async Task Handle(MechanicUserDeletedIntegrationEvent notification, CancellationToken cancellationToken)
    { 
        await _publishEndpoint.Publish(notification, cancellationToken);
    }
}
