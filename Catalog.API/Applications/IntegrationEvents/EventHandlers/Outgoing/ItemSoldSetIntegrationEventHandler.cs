using MassTransit;
using MediatR;
using Catalog.Contracts;

namespace Catalog.API.Applications.IntegrationEvents.EventHandlers.Outgoing;

public class ItemSoldSetIntegrationEventHandler(IPublishEndpoint publishEndpoint) : INotificationHandler<ItemSoldSetIntegrationEvent>
{
    private readonly IPublishEndpoint _publishEndpoint = publishEndpoint; 

    public async Task Handle(ItemSoldSetIntegrationEvent notification, CancellationToken cancellationToken)
    { 
        await _publishEndpoint.Publish(notification, cancellationToken); 
    }
}
