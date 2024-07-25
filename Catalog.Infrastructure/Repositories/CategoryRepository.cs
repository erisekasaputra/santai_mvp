using Catalog.Domain.Aggregates.CategoryAggregate; 
using Microsoft.EntityFrameworkCore;

namespace Catalog.Infrastructure.Repositories;

public class CategoryRepository(CatalogDbContext dbContext) : ICategoryRepository
{
    private readonly CatalogDbContext _context = dbContext; 
    public async Task<Category?> GetCategoryByIdAsync(string id)
    {
        return await _context.Categories.FirstOrDefaultAsync(c => c.Id == id);
    }
}
