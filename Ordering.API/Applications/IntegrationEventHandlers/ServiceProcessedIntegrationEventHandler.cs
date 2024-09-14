using Core.Events; 
using MassTransit;
using MediatR;

namespace Ordering.API.Applications.IntegrationEventHandlers;

public class ServiceProcessedIntegrationEventHandler(IPublishEndpoint publishEndpoint) : INotificationHandler<ServiceProcessedIntegrationEvent>
{
    private readonly IPublishEndpoint _endpoint = publishEndpoint;
    public async Task Handle(ServiceProcessedIntegrationEvent notification, CancellationToken cancellationToken)
    {
        await _endpoint.Publish(notification, cancellationToken);
    }
}
