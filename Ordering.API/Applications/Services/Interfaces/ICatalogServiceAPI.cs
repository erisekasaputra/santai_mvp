using Ordering.API.Applications.Dtos.Responses;

namespace Ordering.API.Applications.Services.Interfaces;

public interface ICatalogServiceAPI
{
    Task<(IEnumerable<CatalogItemsResponseDto>?, string? errorMessage)> SubstractStockAndGetDetailItems(
        IEnumerable<(Guid ItemId, int Quantity)> substractItems);
}
