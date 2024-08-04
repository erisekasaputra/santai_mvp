using Catalog.Contracts; 
using MassTransit;
using MediatR;

namespace Catalog.API.Applications.IntegrationEvents.EventHandlers.Outgoing;

public class ItemDeletedIntegrationEventHandler(IPublishEndpoint publishEndpoint) : INotificationHandler<ItemDeletedIntegrationEvent>
{
    private readonly IPublishEndpoint _publishEndpoint = publishEndpoint;

    public async Task Handle(ItemDeletedIntegrationEvent notification, CancellationToken cancellationToken)
    {  
        await _publishEndpoint.Publish(notification, cancellationToken); 
    }
}
