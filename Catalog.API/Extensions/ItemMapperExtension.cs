
using Catalog.API.Applications.Dtos.Item;
using Catalog.Domain.Aggregates.ItemAggregate; 

namespace Catalog.API.Extensions;

public static class ItemMapperExtension
{
    public static IEnumerable<ItemDto> ToItemsDto(this IEnumerable<Item> items)
    {
        foreach (var item in items)
        {
            yield return item.ToItemDto();
        } 
    }

    public static ItemDto ToItemDto(this Item item)
    { 
        return new ItemDto( 
            item.Id,
            item.Name,
            item.Description,
            item.Sku,
            item.Price.Amount,
            item.Price.Currency,
            item.ImageUrl,
            item.CreatedAt,
            item.StockQuantity,
            item.SoldQuantity,
            item.CategoryId,
            item.Category?.Name ?? string.Empty,
            item.BrandId,
            item.Brand?.Name ?? string.Empty,
            item.IsActive,
            item.OwnerReviews.ToOwnerReviewsDto()!
        );
    }
}
