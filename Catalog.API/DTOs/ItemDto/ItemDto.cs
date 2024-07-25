namespace Catalog.API.DTOs.ItemDto;

public record ItemDto(string Id,
                  string Name,
                  string Description,
                  decimal Price,
                  string ImageUrl,
                  DateTime CreatedAt,
                  int StockQuantity,
                  int SoldQuantity,
                  string CategoryId,
                  string CategoryName)
{   
}
