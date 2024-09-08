 
using MassTransit;
using MediatR; 
using Core.Events;

namespace Catalog.API.Applications.IntegrationEvents.EventHandlers;

public class ItemSoldReducedIntegrationEventHandler(IPublishEndpoint publishEndpoint) : INotificationHandler<ItemSoldReducedIntegrationEvent>
{
    private readonly IPublishEndpoint _publishEndpoint = publishEndpoint;
    public async Task Handle(ItemSoldReducedIntegrationEvent notification, CancellationToken cancellationToken)
    {
        await _publishEndpoint.Publish(notification, cancellationToken);
    }
}
