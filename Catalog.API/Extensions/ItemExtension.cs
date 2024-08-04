using Catalog.API.DTOs.Item; 
using Catalog.Domain.Aggregates.ItemAggregate; 

namespace Catalog.API.Extensions;

public static class ItemExtension
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
            item.Price,
            item.ImageUrl,
            item.CreatedAt,
            item.StockQuantity,
            item.SoldQuantity,
            item.CategoryId ?? string.Empty,
            item.Category?.Name ?? string.Empty,
            item.BrandId ?? string.Empty,
            item.Brand?.Name ?? string.Empty,
            item.IsActive,
            item.OwnerReviews.ToOwnerReviewsDto()!
        );
    }
}
