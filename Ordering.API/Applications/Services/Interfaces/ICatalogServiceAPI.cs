using Ordering.API.Applications.Dtos.Responses;

namespace Ordering.API.Applications.Services.Interfaces;

public interface ICatalogServiceAPI
{
    Task<(ResultResponseDto<List<CatalogItemResponseDto>>? ResultItemResponse, bool IsSuccess)> SubstractStockAndGetDetailItems(
        IEnumerable<(Guid ItemId, int Quantity)> substractItems);
}
