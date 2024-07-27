using Catalog.API.Services;
using Catalog.Contracts;
using Catalog.Domain.Events; 
using MassTransit;
using MediatR; 

namespace Catalog.API.IntegrationEvents.EventHandlers.Outgoing;

public class ItemCreatedIntegrationEventHandler(IPublishEndpoint publishEndpoint, ApplicationService service) : INotificationHandler<ItemCreatedDomainEvent>
{     
    private readonly IPublishEndpoint _publishEndpoint = publishEndpoint;

    private readonly ApplicationService _service = service;
    
    public async Task Handle(ItemCreatedDomainEvent notification, CancellationToken cancellationToken)
    {
        var integrationEvent = new ItemCreatedIntegrationEvent(notification.Item.Id, notification.Item.Name, notification.Item.Description, notification.Item.Price, notification.Item.ImageUrl, notification.Item.CreatedAt, notification.Item.StockQuantity, notification.Item.SoldQuantity, notification.Item.CategoryId, notification.Item.Category.Name, notification.Item.BrandId, notification.Item.Brand.Name);
        
        await _publishEndpoint.Publish(integrationEvent, cancellationToken);

        _service.Logger.LogTrace("Item created : {item}", integrationEvent);
    }
}
