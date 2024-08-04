using Catalog.Contracts; 
using MassTransit;
using MediatR;

namespace Catalog.API.Applications.IntegrationEvents.EventHandlers.Outgoing;

public class ItemUpdatedIntegrationEventHandler(IPublishEndpoint publishEndpoint) : INotificationHandler<ItemUpdatedIntegrationEvent>
{ 
    private readonly IPublishEndpoint _publishEndpoint = publishEndpoint; 

    public async Task Handle(ItemUpdatedIntegrationEvent notification, CancellationToken cancellationToken)
    { 
        await _publishEndpoint.Publish(notification, cancellationToken); 
    }
}
