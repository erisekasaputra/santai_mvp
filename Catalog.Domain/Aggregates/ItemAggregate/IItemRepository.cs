namespace Catalog.Domain.Aggregates.ItemAggregate;

public interface IItemRepository
{ 
    Task<Item> CreateItemAsync(Item item); 
    Task<Item?> GetItemByIdAsync(Guid id); 
    Task<Item?> RetrieveItemById(Guid id); 
    Task<(int TotalCount, int TotalPages, IEnumerable<Item> Items)> GetPaginatedItemsAsync(
        int pageNumber,
        int pageSize,
        Guid? categoryId,
        Guid? brandId,
        bool availableStockOnly = true); 
    void UpdateItem(Item item);  
    Task<ICollection<Item>> GetItemsWithLockAsync(IEnumerable<Guid> itemIds); 
    Task MarkBrandIdToNullByDeletingBrandByIdAsync(Guid brandId); 
    Task MarkCategoryIdToNullByDeletingCategoryByIdAsync(Guid categoryId);
}
