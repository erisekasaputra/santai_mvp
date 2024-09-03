using Catalog.Contracts;
using Catalog.Domain.Events;
using MediatR;

namespace Catalog.API.Applications.DomainEventHandlers;

public class ItemUpdatedDomainEventHandler(IMediator mediator) : INotificationHandler<ItemUpdatedDomainEvent>
{
    private readonly IMediator _mediator = mediator;

    public async Task Handle(ItemUpdatedDomainEvent notification, CancellationToken cancellationToken)
    {
        var item = notification.Item;

        var ownerReviews = item.OwnerReviews.Select(item => new OwnerReviewIntegrationEvent(item.Title, item.Rating));

        var @event = new ItemUpdatedIntegrationEvent(
            item.Id,
            item.Name,
            item.Description,
            item.Sku,   
            item.Price,
            item.ImageUrl,
            item.CreatedAt,
            item.StockQuantity,
            item.SoldQuantity,
            item.CategoryId,
            item.Category?.Name,
            item.Category?.ImageUrl,
            item.BrandId,
            item.Brand?.Name,
            item.Brand?.ImageUrl,
            item.IsActive,
            item.IsDeleted,
            ownerReviews);

        await _mediator.Publish(@event, cancellationToken);
    }
}
