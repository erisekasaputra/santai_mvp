using Core.Enumerations;

namespace Catalog.API.Applications.Dtos.ItemPrice;

public record ItemPriceDto(
    Guid ItemId,
    decimal Amount,
    Currency? Currency,
    string Message);
