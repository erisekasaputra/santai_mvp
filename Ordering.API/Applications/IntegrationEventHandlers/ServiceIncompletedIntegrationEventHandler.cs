using Core.Events.Ordering;
using MassTransit;
using MediatR;

namespace Ordering.API.Applications.IntegrationEventHandlers;

public class ServiceIncompletedIntegrationEventHandler(IPublishEndpoint publishEndpoint) : INotificationHandler<ServiceIncompletedIntegrationEvent>
{
    private readonly IPublishEndpoint _endpoint = publishEndpoint;
    public async Task Handle(ServiceIncompletedIntegrationEvent notification, CancellationToken cancellationToken)
    {
        await _endpoint.Publish(notification, cancellationToken);
    }
}
