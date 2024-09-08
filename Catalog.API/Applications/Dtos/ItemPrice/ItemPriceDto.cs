namespace Catalog.API.Applications.Dtos.ItemPrice;

public record ItemPriceDto(
    Guid ItemId,
    decimal Amount,
    string Message);
