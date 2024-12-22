using Catalog.Domain.Aggregates.ItemAggregate; 
using Catalog.Infrastructure.Helpers; 
using Microsoft.EntityFrameworkCore; 

namespace Catalog.Infrastructure.Repositories;

public class ItemRepository(CatalogDbContext context, MetaTableHelper metaTableHelper) : IItemRepository
{
    private readonly CatalogDbContext _context = context;

    private readonly MetaTableHelper _metaTableHelper = metaTableHelper;

    public async Task<Item> CreateItemAsync(Item item)
    {
        var result = await _context.Items.AddAsync(item);
        return result.Entity;
    }

    public async Task<Item?> RetrieveItemById(Guid id)
    {
        return await _context.Items
            .Where (x => !x.IsDeleted)
            .Include(p => p.Category)
            .Include(p => p.Brand) 
            .FirstOrDefaultAsync(e => e.Id == id);
    }
  
    public async Task<Item?> GetItemByIdAsync(Guid id)
    {
        return await _context.Items
            .Include(p => p.Category)
            .Include(p => p.Brand) 
            .FirstOrDefaultAsync(e => e.Id == id);
    }

    public async Task<(int TotalCount, int TotalPages, IEnumerable<Item> Items)> GetPaginatedItemsAsync(
        int pageNumber,
        int pageSize,
        Guid? categoryId,
        Guid? brandId,
        bool availableStockOnly = true)
    {
        var query = _context.Items
            .Where(w => !w.IsDeleted)
            .AsQueryable(); 
        if (categoryId is not null && categoryId.HasValue && categoryId != Guid.Empty) 
        {
            query = query.Where(x => x.CategoryId == categoryId);
        }

        if (brandId is not null && brandId.HasValue && brandId != Guid.Empty)
        {
            query = query.Where(x => x.BrandId == brandId);
        }

        if (availableStockOnly)
        {
            query = query.Where(x => x.StockQuantity > 0);
            query = query.Where(x => x.IsActive);
        }

        var totalCount = await query.CountAsync(); 
        var totalPages = (int) Math.Ceiling(totalCount / (double)pageSize); 
        var items = await query
            .OrderBy(x => x.Name)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .Include(d => d.Category)
            .Include(d => d.Brand)
            .AsNoTracking()
            .ToListAsync();

        return (totalCount, totalPages, items);
    }

    public void UpdateItem(Item item)
    { 
        _context.Items.Update(item);
    } 

    public async Task<ICollection<Item>> GetItemsWithLockAsync(IEnumerable<Guid> itemIds)
    { 
        return await _context.Items.Where(x => itemIds.Contains(x.Id)).ToListAsync(); 
    }

    public async Task MarkBrandIdToNullByDeletingBrandByIdAsync(Guid brandId)
    {
        Guid? nulling = null;
        await _context.Items.Where(x => x.BrandId == brandId)
            .ExecuteUpdateAsync(u => u.SetProperty(p => p.BrandId, nulling));
    } 
    public async Task MarkCategoryIdToNullByDeletingCategoryByIdAsync(Guid categoryId)
    {
        Guid? nulling = null;
        await _context.Items.Where(x => x.CategoryId == categoryId)
            .ExecuteUpdateAsync(u => 
                u.SetProperty(p => p.CategoryId, nulling));
    } 
}
