using Core.Events; 
using MassTransit;
using MediatR;

namespace Ordering.API.Applications.IntegrationEventHandlers;

public class MechanicAssignedIntegrationEventHandler(IPublishEndpoint publishEndpoint) : INotificationHandler<MechanicAssignedIntegrationEvent>
{
    private readonly IPublishEndpoint _endpoint = publishEndpoint;
    public async Task Handle(MechanicAssignedIntegrationEvent notification, CancellationToken cancellationToken)
    {
        await _endpoint.Publish(notification, cancellationToken);
    }
}
