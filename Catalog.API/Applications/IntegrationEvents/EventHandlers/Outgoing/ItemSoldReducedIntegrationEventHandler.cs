using Catalog.API.Services;
using Catalog.Domain.Events;
using MassTransit;
using MediatR;
using Catalog.Contracts;

namespace Catalog.API.Applications.IntegrationEvents.EventHandlers.Outgoing;

public class ItemSoldReducedIntegrationEventHandler(IPublishEndpoint publishEndpoint) : INotificationHandler<ItemSoldReducedIntegrationEvent>
{
    private readonly IPublishEndpoint _publishEndpoint = publishEndpoint; 
    public async Task Handle(ItemSoldReducedIntegrationEvent notification, CancellationToken cancellationToken)
    { 
        await _publishEndpoint.Publish(notification, cancellationToken); 
    }
}
