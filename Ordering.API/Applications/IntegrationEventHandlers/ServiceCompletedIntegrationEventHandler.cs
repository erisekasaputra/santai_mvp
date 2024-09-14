using Core.Events; 
using MassTransit;
using MediatR;

namespace Ordering.API.Applications.IntegrationEventHandlers;

public class ServiceCompletedIntegrationEventHandler(IPublishEndpoint publishEndpoint) : INotificationHandler<ServiceCompletedIntegrationEvent>
{
    private readonly IPublishEndpoint _endpoint = publishEndpoint;
    public async Task Handle(ServiceCompletedIntegrationEvent notification, CancellationToken cancellationToken)
    {
        await _endpoint.Publish(notification, cancellationToken);
    }
}
