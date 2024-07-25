using Catalog.API.DTOs.ItemDto; 
using Catalog.Domain.Aggregates.ItemAggregate;
using System.Linq;

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
            item.CategoryId,
            item.Category.Name
        );
    }
}
