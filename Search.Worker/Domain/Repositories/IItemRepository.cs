using Search.Worker.Domain.Models;

namespace Search.Worker.Domain.Repositories;

public interface IItemRepository
{
    Task<Item?> GetItemByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<bool> CreateItemAsync(Item item, CancellationToken cancellationToken = default);
    Task<bool> DeleteItemAsync(Item item, CancellationToken cancelToken = default);
    Task<bool> UpdateItemAsync(Item item, CancellationToken cancelToken = default);
    Task<bool> UpdateCategoryByCategoryIdAsync(Guid Id, string Name, string ImageUrl);
    Task<bool> UpdateBrandByBrandIdAsync(Guid Id, string Name, string ImageUrl);
    Task<bool> DeleteCategoryByCategoryIdAsync(Guid Id);
    Task<bool> DeleteBrandByBrandIdAsync(Guid Id);
}

