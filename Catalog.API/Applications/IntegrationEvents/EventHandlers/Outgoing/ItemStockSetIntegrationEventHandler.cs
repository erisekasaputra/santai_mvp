using MassTransit;
using MediatR;
using Catalog.Contracts;

namespace Catalog.API.Applications.IntegrationEvents.EventHandlers.Outgoing;

public class ItemStockSetIntegrationEventHandler(IPublishEndpoint publishEndpoint) : INotificationHandler<ItemStockSetIntegrationEvent>
{
    private readonly IPublishEndpoint _publishEndpoint = publishEndpoint; 

    public async Task Handle(ItemStockSetIntegrationEvent notification, CancellationToken cancellationToken)
    { 
        await _publishEndpoint.Publish(notification, cancellationToken); 
    }
}
