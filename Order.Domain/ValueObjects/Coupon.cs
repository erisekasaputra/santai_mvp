using Order.Domain.Enumerations;
using Order.Domain.Exceptions;
using Order.Domain.SeedWork;

namespace Order.Domain.ValueObjects; 

public class Coupon : ValueObject
{
    public string CouponCode { get; private set; }
    public PercentageOrValueType CouponValueType { get; private set; }
    public decimal? Percentage { get; private set; }
    public Money? Value { get; private set; }
    public Money MinimumOrderValue { get; private set; }

    private Coupon(
        string couponCode,
        PercentageOrValueType couponValueType,
        decimal percentage,
        Money? value,
        Money minimumOrderValue)
    {
        if (couponValueType == PercentageOrValueType.Percentage && (percentage < 1 || percentage > 100))
            throw new DomainException("Discount percentage must be between 1 and 100.");

        if (couponValueType == PercentageOrValueType.Value && (value is null || value.Amount < 1))
            throw new DomainException("Discount value cannot be zero or negative.");

        if (minimumOrderValue.Amount < 0)
            throw new DomainException("Minimum order value cannot be negative.");

        CouponCode = couponCode;
        CouponValueType = couponValueType;
        Percentage = couponValueType == PercentageOrValueType.Percentage ? percentage : 0;
        Value = couponValueType == PercentageOrValueType.Value ? value : null;
        MinimumOrderValue = minimumOrderValue;
    }

    public static Coupon CreatePercentageDiscount(
        string couponCode,
        decimal percentage,
        Money minimumOrderValue)
    {
        return new Coupon(couponCode, PercentageOrValueType.Percentage, percentage, null, minimumOrderValue);
    }

    public static Coupon CreateValueDiscount(
        string couponCode,
        Money value,
        Money minimumOrderValue)
    {
        return new Coupon(couponCode, PercentageOrValueType.Value, 0, value, minimumOrderValue);
    }

    public Money Apply(Money orderAmount)
    {
        if (orderAmount.Amount < 0)
            throw new DomainException("Amount cannot be negative.");

        if (orderAmount.Amount < MinimumOrderValue.Amount)
            return new Money(0, orderAmount.Currency);

        Money discountAmount;

        if (CouponValueType == PercentageOrValueType.Percentage && Percentage > 0 && Percentage.HasValue && Percentage.Value > 0)
        {
            var percentageAmount = new Money(orderAmount.Amount * (Percentage.Value / 100), orderAmount.Currency);
            discountAmount = percentageAmount;
        }
        else if (CouponValueType == PercentageOrValueType.Value && Value is not null)
        {
            discountAmount = new Money(Value.Amount, Value.Currency);
        }
        else
        {
            discountAmount = new Money(0, orderAmount.Currency);
        }

        return discountAmount > orderAmount ? throw new DomainException("Discount amount can not greater than order amount") : discountAmount;
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return CouponCode;
        yield return CouponValueType;
        yield return Percentage;
        yield return Value;
        yield return MinimumOrderValue;
    }
}