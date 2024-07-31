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
        var itemCreated = notification.Item;

        var ownerReview = itemCreated.OwnerReviews.Select(item =>
        { 
            return new OwnerReviewIntegrationEvent(item.Title, item.Rating);
        });

        var integrationEvent = new ItemCreatedIntegrationEvent(itemCreated.Id, itemCreated.Name, itemCreated.Description, itemCreated.Price, itemCreated.ImageUrl, itemCreated.CreatedAt, itemCreated.StockQuantity, itemCreated.SoldQuantity, itemCreated.CategoryId, itemCreated.Category.Name, itemCreated.BrandId, itemCreated.Brand.Name, ownerReview);
        
        await _publishEndpoint.Publish(integrationEvent, cancellationToken);

        _service.Logger.LogTrace("Item created : {item}", integrationEvent);
    }
}
