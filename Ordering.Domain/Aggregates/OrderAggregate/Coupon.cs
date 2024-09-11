using Core.Enumerations;
using Core.Exceptions;
using Core.ValueObjects;
using Ordering.Domain.Enumerations;
using Ordering.Domain.SeedWork;

namespace Ordering.Domain.Aggregates.OrderAggregate;
public class Coupon : Entity
{
    public Guid OrderingId { get; private set; }
    public string CouponCode { get; private set; }
    public PercentageOrValueType CouponValueType { get; private set; }
    public Currency Currency { get; private set; }
    public decimal ValuePercentage { get; private set; }
    public decimal ValueAmount { get; private set; }
    public decimal MinimumOrderValue { get; private set; }
    public Money DiscountAmount { get; private set; }

    public Coupon()
    {
        CouponCode = string.Empty;
        DiscountAmount = null!;
    }

    private Coupon(
        Guid orderingId,
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
        OrderingId = orderingId;
        CouponCode = couponCode;
        CouponValueType = couponValueType;
        ValuePercentage = couponValueType == PercentageOrValueType.Percentage ? value : 0;
        ValueAmount = couponValueType == PercentageOrValueType.Value ? value : 0;
        MinimumOrderValue = minimumOrderValue;
        DiscountAmount = new Money(0, currency);
    }

    public static Coupon CreatePercentageDiscount(
        Guid orderingId,
        string couponCode,
        decimal percentage,
        decimal minimumOrderAmount,
        Currency currency)
    {
        return new Coupon(orderingId, couponCode, PercentageOrValueType.Percentage, currency, percentage, minimumOrderAmount);
    }

    public static Coupon CreateValueDiscount(
        Guid orderingId,
        string couponCode,
        decimal amount,
        Currency currency,
        decimal minimumOrderAmount)
    {
        return new Coupon(orderingId, couponCode, PercentageOrValueType.Value, currency, amount, minimumOrderAmount);
    }

    public Money Apply(decimal orderAmount, Currency orderCurrency)
    {
        if (orderAmount < 0)
            throw new DomainException("Amount cannot be negative.");

        if (orderAmount < MinimumOrderValue)
            return new Money(0, orderCurrency);

        if (orderCurrency != Currency)
        {
            throw new DomainException("Order amount currency should be the same as discount amount currency");
        }

        Money discountAmount;

        if (CouponValueType == PercentageOrValueType.Percentage && ValuePercentage > 0)
        {
            var percentageAmount = new Money(orderAmount * (ValuePercentage / 100), orderCurrency);
            discountAmount = percentageAmount;
        }
        else if (CouponValueType == PercentageOrValueType.Value && ValueAmount > 0)
        {
            discountAmount = new Money(ValueAmount, Currency);
        }
        else
        {
            discountAmount = new Money(0, orderCurrency);
        }

        DiscountAmount = discountAmount;

        return discountAmount.Amount > orderAmount ? throw new DomainException("Discount amount can not greater than order amount") : discountAmount;
    }
}