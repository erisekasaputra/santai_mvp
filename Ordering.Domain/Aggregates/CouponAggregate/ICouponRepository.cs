namespace Ordering.Domain.Aggregates.CouponAggregate;

public interface ICouponRepository
{
    Task CreateAsync(Coupon coupon);
    void Update(Coupon coupon);
    void Delete(Coupon coupon);
    Task<Coupon?> GetByIdAsync(Guid id);
    Task<Coupon?> GetByCodeAsync(string code);
    Task<bool> GetAnyByIdOrCodeAsync(Guid id, string code);
    Task<(int TotalCount, int TotalPages, IEnumerable<Coupon> Coupons)> GetPaginatedCoupons(int pageNumber, int pageSize);
}
