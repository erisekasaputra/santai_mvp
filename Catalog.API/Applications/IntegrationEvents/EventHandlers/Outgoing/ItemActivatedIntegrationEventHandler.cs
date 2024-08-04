using Catalog.API.Services;
using Catalog.Contracts; 
using MassTransit;
using MediatR;

namespace Catalog.API.Applications.IntegrationEvents.EventHandlers.Outgoing;

public class ItemActivatedIntegrationEventHandler(IPublishEndpoint publishEndpoint) : INotificationHandler<ItemActivatedIntegrationEvent>
{
    private readonly IPublishEndpoint _publishEndpoint = publishEndpoint; 

    public async Task Handle(ItemActivatedIntegrationEvent notification, CancellationToken cancellationToken)
    { 
        await _publishEndpoint.Publish(notification, cancellationToken); 
    }
}
