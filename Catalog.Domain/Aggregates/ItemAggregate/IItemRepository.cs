namespace Catalog.Domain.Aggregates.ItemAggregate;

public interface IItemRepository
{
    Task<Item> CreateItemAsync(Item item);

    Task<Item?> GetItemByIdAsync(string id);

    Task<(int TotalCount, int TotalPages, IEnumerable<Item> Items)> GetPaginatedItemsAsync(int pageNumber, int pageSize);

    void UpdateItem(Item item);

    void DeleteItem(Item item);
}
