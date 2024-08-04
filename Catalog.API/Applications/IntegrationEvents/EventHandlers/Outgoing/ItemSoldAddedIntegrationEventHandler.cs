using MassTransit;
using MediatR;
using Catalog.Contracts;

namespace Catalog.API.Applications.IntegrationEvents.EventHandlers.Outgoing;

public class ItemSoldAddedIntegrationEventHandler(IPublishEndpoint publishEndpoint) : INotificationHandler<ItemSoldAddedIntegrationEvent>
{
    private readonly IPublishEndpoint _publishEndpoint = publishEndpoint;

    public async Task Handle(ItemSoldAddedIntegrationEvent notification, CancellationToken cancellationToken)
    { 
        await _publishEndpoint.Publish(notification, cancellationToken); 
    }
}
