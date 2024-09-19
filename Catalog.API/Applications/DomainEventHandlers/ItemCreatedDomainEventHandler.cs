 
using Catalog.Domain.Events;
using Core.Events;
using MediatR;

namespace Catalog.API.Applications.DomainEventHandlers;

public class ItemCreatedDomainEventHandler(IMediator mediator) : INotificationHandler<ItemCreatedDomainEvent>
{
    private readonly IMediator _mediator = mediator;

    public async Task Handle(ItemCreatedDomainEvent notification, CancellationToken cancellationToken)
    {
        var item = notification.Item; 

        var ownerReview = item.OwnerReviews.Select(item => new OwnerReviewIntegrationEvent(item.Title, item.Rating));

        var @event = new ItemCreatedIntegrationEvent(
            item.Id,
            item.Name,
            item.Description,
            item.Sku,
            item.LastPrice,
            item.Price.Amount,
            item.Price.Currency,
            item.ImageUrl, 
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
            ownerReview);

        await _mediator.Publish(@event, cancellationToken);
    }
}
