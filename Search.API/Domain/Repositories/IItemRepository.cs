using Search.API.Applications.Dto;
using Search.API.Domain.Models;

namespace Search.API.Domain.Repositories;

public interface IItemRepository
{
    Task<IEnumerable<Item>> GetPaginatedBySearchQueryAsync(SearchRequestDto query);
}
