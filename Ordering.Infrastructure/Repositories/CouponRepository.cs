using MassTransit.Internals.GraphValidation;
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

    public void DeleteAsync(Coupon coupon)
    {
        _context.Remove(coupon);
    }

    public async Task<Coupon?> GetByCodeAsync(string code)
    {
        return await _context.Coupons.Where(x => x.CouponCode == code).FirstOrDefaultAsync();
    }

    public async Task<Coupon?> GetByIdAsync(Guid id)
    {
        return await _context.Coupons.Where(x => x.Id == id).FirstOrDefaultAsync();
    }

    public void UpdateAsync(Coupon coupon)
    { 
        _context.Coupons.Update(coupon);
    }
}
