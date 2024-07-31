using Catalog.Domain.Aggregates.CategoryAggregate;
using Catalog.Infrastructure.Helpers;
using Microsoft.EntityFrameworkCore;

namespace Catalog.Infrastructure.Repositories;

public class CategoryRepository(CatalogDbContext dbContext, MetaTableHelper metaTableHelper) : ICategoryRepository
{
    private readonly CatalogDbContext _context = dbContext; 
    private readonly MetaTableHelper _metaTableHelper = metaTableHelper;
    public async Task<Category?> GetCategoryByIdAsync(string id)
    {
        return await _context.Categories.FirstOrDefaultAsync(c => c.Id == id);
    }
     

    public async Task<Category> CreateCategoryAsync(Category item)
    {
        var entry = await _context.Categories.AddAsync(item);

        return entry.Entity;
    }

    public void DeleteCategory(Category item)
    {
        _context.Categories.Remove(item);
    }
     

    public async Task<(int TotalCount, int TotalPages, IEnumerable<Category> Categories)> GetPaginatedCategoriesAsync(int pageNumber, int pageSize)
    {
        var query = _context.Categories.AsQueryable();

        var totalCount = await query.CountAsync();

        var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

        var items = await query.Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .AsNoTracking()
            .OrderBy(x => x.Name)
            .ToListAsync();

        return (totalCount, totalPages, items);
    }

    public void UpdateCategory(Category item)
    {
        _context.Categories.Update(item);
    }
     
    /// <summary>
    /// Returning single record or default for Category data    
    /// </summary>
    /// <param name="name">the name will be parsed, trimmed, and to lowered</param>
    /// <returns></returns>
    public Task<Category?> GetCategoryByNameAsync(string name)
    {
        var category = _context.Categories.FirstOrDefaultAsync(x => x.Name.ToLower().Trim() == name.ToLower().Trim()); 
        return category;
    }
}
