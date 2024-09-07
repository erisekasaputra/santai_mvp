using Order.Domain.Enumerations;
using Order.Domain.Exceptions;
using Order.Domain.SeedWork;
using Order.Domain.ValueObjects;

namespace Order.Domain.Aggregates.OrderAggregate;

public class Coupon : Entity
{
    public Guid OrderingId { get; private set; }
    public string CouponCode { get; private set; }
    public PercentageOrValueType CouponValueType { get; private set; }
    public decimal? Percentage { get; private set; }
    public Money? Value { get; private set; }
    public Money MinimumOrderValue { get; private set; }
    public Money DiscountAmount { get; private set; }

    private Coupon(
        Guid orderingId,
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

        if (couponValueType == PercentageOrValueType.Value && value is not null && value.Currency != minimumOrderValue.Currency)
            throw new DomainException("Coupon value currency should be the same as minimum order currency");

        OrderingId = orderingId;
        CouponCode = couponCode;
        CouponValueType = couponValueType;
        Percentage = couponValueType == PercentageOrValueType.Percentage ? percentage : 0;
        Value = couponValueType == PercentageOrValueType.Value ? value : null;
        MinimumOrderValue = minimumOrderValue;
        DiscountAmount = new Money(0, minimumOrderValue.Currency);
    }

    public static Coupon CreatePercentageDiscount(
        Guid orderingId,
        string couponCode,
        decimal percentage,
        decimal minimumOrderAmount,
        Currency minimumOrderCurrency)
    {
        return new Coupon(orderingId, couponCode, PercentageOrValueType.Percentage, percentage, null, new Money(minimumOrderAmount, minimumOrderCurrency));
    }

    public static Coupon CreateValueDiscount(
        Guid orderingId,
        string couponCode,
        decimal amount,
        Currency currency,
        decimal minimumOrderAmount)
    {
        return new Coupon(orderingId, couponCode, PercentageOrValueType.Value, 0, new Money(amount, currency), new Money(minimumOrderAmount, currency));
    }

    public Money Apply(Money orderAmount)
    {
        if (orderAmount.Amount < 0)
            throw new DomainException("Amount cannot be negative.");

        if (orderAmount.Amount < MinimumOrderValue.Amount)
            return new Money(0, orderAmount.Currency);

        if (orderAmount.Currency != DiscountAmount.Currency)
        {
            throw new DomainException("Order amount currency should be the same as discount amount currency");
        }

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

        DiscountAmount = discountAmount;

        return discountAmount > orderAmount ? throw new DomainException("Discount amount can not greater than order amount") : discountAmount;
    }
}