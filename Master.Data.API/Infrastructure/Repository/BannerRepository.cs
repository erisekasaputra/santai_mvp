using Master.Data.API.Domain.Entity;
using Microsoft.EntityFrameworkCore;

namespace Master.Data.API.Infrastructure.Repository;

public class BannerRepository
{
    private readonly MasterDbContext _context;
    public BannerRepository(MasterDbContext context)
    {
        _context = context;
    }

    public async Task<Banner> SaveAsync(Banner banner)
    {
        var result = await _context.Banners.AddAsync(banner);
        await _context.SaveChangesAsync();

        return result.Entity; 
    }

    public async Task UpdateAsync(Banner banner)
    {
        _context.Banners.Update(banner);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Banner banner)
    {
        _context.Banners.Remove(banner);    
        await _context.SaveChangesAsync();
    }

    public async Task<List<Banner>> GetActiveBannersAsync()
    {
        return await _context.Banners.Where(x => x.IsActive == true).ToListAsync();
    }

    public async Task<Banner?> GetBannerById(Guid id)
    {
        return await _context.Banners.FirstOrDefaultAsync(x => x.Id == id);
    } 

    public async Task<(int TotalCount, int TotalPages, IEnumerable<Banner> Banners)> GetPaginatedBanners(int pageNumber, int pageSize)
    {
        var query = _context.Banners.AsQueryable();

        var totalCount = await query.CountAsync();

        var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

        var items = await query.Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .AsNoTracking()
            .OrderBy(x => x.Name)
            .ToListAsync();

        return (totalCount, totalPages, items);
    }
}
