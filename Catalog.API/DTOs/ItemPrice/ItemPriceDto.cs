namespace Catalog.API.DTOs.ItemPrice;

public record ItemPriceDto(
    Guid ItemId,
    decimal Amount,
    string Message);
