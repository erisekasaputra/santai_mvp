namespace Catalog.API.DTOs.ItemStock;

public record ItemStockDto(Guid ItemId, int Quantity, string Message);
