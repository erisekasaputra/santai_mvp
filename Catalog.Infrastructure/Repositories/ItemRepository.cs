using Catalog.Domain.Aggregates.ItemAggregate; 
using Catalog.Infrastructure.Helpers;
using Microsoft.Data.SqlClient;
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
  
    public async Task<Item?> GetItemByIdAsync(string id)
    {
        return await _context.Items
            .Include(p => p.Category)
            .Include(p => p.Brand)
            .Include(p => p.OwnerReviews)
            .FirstOrDefaultAsync(e => e.Id == id);
    }

    public async Task<(int TotalCount, int TotalPages, IEnumerable<Item> Items)> GetPaginatedItemsAsync(int pageNumber, int pageSize)
    {
        var query = _context.Items.AsQueryable();

        var totalCount = await query.CountAsync();

        var totalPages = (int) Math.Ceiling(totalCount / (double)pageSize);

        var items = await query.Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .Include(d => d.Category)
            .AsNoTracking()
            .OrderBy(x => x.Name)
            .ToListAsync();

        return (totalCount, totalPages, items);
    }

    public void UpdateItem(Item item)
    { 
        _context.Items.Update(item);
    }
    public void DeleteItem(Item item)
    {
        item.Delete();
        _context.Items.Remove(item);
    }

    public async Task<ICollection<Item>> GetItemsWithLockAsync(IEnumerable<string> itemIds)
    {
        var tableName = _metaTableHelper.GetTableName<Item>();
        var itemIdColumn = _metaTableHelper.GetColumnName<Item>(nameof(Item.Id)); 

        var parameterNames = itemIds.Select((id, index) =>
        {
            return $"@p{index}";

        }).ToArray();

        var sql = @$"SELECT *
                        FROM {tableName} WITH (UPDLOCK, ROWLOCK) WHERE {itemIdColumn} IN ({string.Join(", ", parameterNames)})";

        var parameters = itemIds.Select((id, index) =>
        { 
            return new SqlParameter($"@p{index}", id);

        }).ToArray();
         
        return await _context.Items.FromSqlRaw(sql, parameters).ToListAsync(); 
    }
}
