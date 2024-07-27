using Search.Worker.Domain.Models;

namespace Search.Worker.Domain.Repository;

public interface IItemRepository
{ 
    Task<Item?> GetItemById(string id, CancellationToken cancellationToken = default);
    Task<bool> CreateItemAsync(Item item, CancellationToken cancellationToken = default); 
    Task<bool> DeleteItemAsync(Item item, CancellationToken cancelToken = default);
    Task<bool> UpdateItemAsync(Item item, CancellationToken cancelToken = default);
}
