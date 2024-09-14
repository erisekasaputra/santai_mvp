namespace Ordering.Domain.Aggregates.CouponAggregate;

public interface ICouponRepository
{
    Task CreateAsync(Coupon coupon);
    void UpdateAsync(Coupon coupon);
    void DeleteAsync(Coupon coupon);
    Task<Coupon?> GetByIdAsync(Guid id);
    Task<Coupon?> GetByCodeAsync(string code);
}
