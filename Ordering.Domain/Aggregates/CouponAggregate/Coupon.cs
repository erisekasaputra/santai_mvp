using Core.Enumerations;
using Core.Exceptions; 
using Ordering.Domain.Enumerations;
using Ordering.Domain.SeedWork;

namespace Ordering.Domain.Aggregates.CouponAggregate;

public class Coupon : Entity
{ 
    public string CouponCode { get; private set; }
    public PercentageOrValueType CouponValueType { get; private set; }
    public Currency Currency { get; private set; }
    public decimal ValuePercentage { get; private set; }
    public decimal ValueAmount { get; private set; }
    public decimal MinimumOrderValue { get; private set; } 

    public Coupon()
    {
        CouponCode = string.Empty; 
    }

    public Coupon( 
        string couponCode,
        PercentageOrValueType couponValueType,
        Currency currency,
        decimal value,
        decimal minimumOrderValue)
    {
        if (couponValueType == PercentageOrValueType.Percentage && (value < 1.00M || value > 100.00M))
            throw new DomainException("Discount percentage must be between 1 and 100.");

        if (couponValueType == PercentageOrValueType.Value && value < 1)
            throw new DomainException("Discount value cannot be zero or negative.");

        if (minimumOrderValue < 0)
            throw new DomainException("Minimum order value cannot be negative.");

        Currency = currency; 
        CouponCode = couponCode;
        CouponValueType = couponValueType;
        ValuePercentage = couponValueType == PercentageOrValueType.Percentage ? value : 0;
        ValueAmount = couponValueType == PercentageOrValueType.Value ? value : 0;
        MinimumOrderValue = minimumOrderValue; 
    } 

    public void Update(
        string couponCode,
        PercentageOrValueType couponValueType,
        Currency currency,
        decimal value,
        decimal minimumOrderValue)
    {
        if (couponValueType == PercentageOrValueType.Percentage && (value < 1.00M || value > 100.00M))
            throw new DomainException("Discount percentage must be between 1 and 100.");

        if (couponValueType == PercentageOrValueType.Value && value < 1)
            throw new DomainException("Discount value cannot be zero or negative.");

        if (minimumOrderValue < 0)
            throw new DomainException("Minimum order value cannot be negative.");

        Currency = currency;
        CouponCode = couponCode;
        CouponValueType = couponValueType;
        ValuePercentage = couponValueType == PercentageOrValueType.Percentage ? value : 0;
        ValueAmount = couponValueType == PercentageOrValueType.Value ? value : 0;
        MinimumOrderValue = minimumOrderValue;
    }
}
