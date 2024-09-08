using Core.Events; 
using MassTransit;
using MediatR;

namespace Account.API.Applications.IntegrationEventHandlers;

public class BusinessUserDeletedIntegrationEventHandler(IPublishEndpoint publishEndpoint) : INotificationHandler<BusinessUserDeletedIntegrationEvent>
{
    private readonly IPublishEndpoint _publishEndpoint = publishEndpoint;
    public async Task Handle(BusinessUserDeletedIntegrationEvent notification, CancellationToken cancellationToken)
    {
        await _publishEndpoint.Publish(notification, cancellationToken); 
    }
}
