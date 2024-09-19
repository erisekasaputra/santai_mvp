using Microsoft.EntityFrameworkCore;
using Ordering.Domain.Aggregates.CouponAggregate;

namespace Ordering.Infrastructure.Repositories;

public class CouponRepository : ICouponRepository
{
    private readonly OrderDbContext _context;
    public CouponRepository(OrderDbContext context)
    {
        _context = context;
    }

    public async Task CreateAsync(Coupon coupon)
    {
        await _context.Coupons.AddAsync(coupon);    
    }

    public void Delete(Coupon coupon)
    {
        _context.Remove(coupon);
    }

    public async Task<bool> GetAnyByIdOrCodeAsync(Guid id, string code)
    {
        return await _context.Coupons.Where(x => x.Id == id || x.CouponCode == code).AnyAsync();    
    }

    public async Task<Coupon?> GetByCodeAsync(string code)
    {
        return await _context.Coupons.Where(x => x.CouponCode == code).FirstOrDefaultAsync();
    }

    public async Task<Coupon?> GetByIdAsync(Guid id)
    {
        return await _context.Coupons.Where(x => x.Id == id).FirstOrDefaultAsync();
    }

    public async Task<(int TotalCount, int TotalPages, IEnumerable<Coupon> Coupons)> GetPaginatedCoupons(int pageNumber, int pageSize)
    {
        var query = _context.Coupons.AsQueryable();  
        var totalCount = await query.CountAsync(); 
        var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize); 
        var items = await query
            .AsNoTracking() 
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(); 
        return (totalCount, totalPages, items);
    }

    public void Update(Coupon coupon)
    { 
        _context.Coupons.Update(coupon);
    }
}
