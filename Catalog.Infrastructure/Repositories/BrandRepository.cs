using Catalog.Domain.Aggregates.BrandAggregate;
using Microsoft.EntityFrameworkCore;

namespace Catalog.Infrastructure.Repositories;

public class BrandRepository(CatalogDbContext context) : IBrandRepository
{
    private readonly CatalogDbContext _context = context; 

    public async Task<Brand> CreateBrandAsync(Brand item)
    {  
        var entry = await _context.Brands.AddAsync(item);

        return entry.Entity; 
    }

    public void DeleteBrand(Brand item)
    {
        _context.Brands.Remove(item);
    }

    public async Task<Brand?> GetBrandByIdAsync(string id)
    {
        return await _context.Brands.FirstOrDefaultAsync(x => x.Id == id);    
    }

    public async Task<(int TotalCount, int TotalPages, IEnumerable<Brand> Brands)> GetPaginatedBrandsAsync(int pageNumber, int pageSize)
    {
        var query = _context.Brands.AsQueryable();

        var totalCount = await query.CountAsync();

        var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

        var items = await query.Skip((pageNumber - 1) * pageSize)
            .Take(pageSize) 
            .AsNoTracking()
            .OrderBy(x => x.Name)
            .ToListAsync();

        return (totalCount, totalPages, items);
    }

    public void UpdateBrand(Brand item)
    {
        _context.Brands.Update(item);
    }

    public async Task<Brand?> GetBrandByNameAsync(string name)
    {
        return await _context.Brands.FirstOrDefaultAsync(x => x.Name.ToLower().Trim() == name.ToLower().Trim());
    }
}
