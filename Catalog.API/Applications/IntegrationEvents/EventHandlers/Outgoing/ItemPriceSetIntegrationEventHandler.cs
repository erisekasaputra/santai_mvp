using Catalog.Contracts; 
using MassTransit;
using MediatR;

namespace Catalog.API.Applications.IntegrationEvents.EventHandlers.Outgoing;

public class ItemPriceSetIntegrationEventHandler(IPublishEndpoint publishEndpoint) : INotificationHandler<ItemPriceSetIntegrationEvent>
{
    private readonly IPublishEndpoint _publishEndpoint = publishEndpoint; 

    public async Task Handle(ItemPriceSetIntegrationEvent notification, CancellationToken cancellationToken)
    { 
        await _publishEndpoint.Publish(notification, cancellationToken); 
    }
}
