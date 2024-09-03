using Order.Domain.Enumerations;
using Order.Domain.Exceptions;
using Order.Domain.SeedWork;

namespace Order.Domain.ValueObjects;

public class Discount : ValueObject
{
    public string DiscountCode { get; private set; }
    public DiscountType DiscountType { get; private set; }
    public decimal Percentage { get; private set; }
    public decimal Value { get; private set; }
    public decimal MinimumOrderValue { get; private set; }

    private Discount(
        string discountCode,
        DiscountType discountType,
        decimal percentage,
        decimal value,
        decimal minimumOrderValue)
    {
        if (discountType == DiscountType.Percentage && (percentage < 1 || percentage > 100))
            throw new DomainException("Discount percentage must be between 1 and 100.");

        if (discountType == DiscountType.Value && (value < 1))
            throw new DomainException("Discount value cannot be zero or negative.");

        if (minimumOrderValue < 0)
            throw new DomainException("Minimum order value cannot be negative.");

        DiscountCode = discountCode;
        DiscountType = discountType;
        Percentage = DiscountType == DiscountType.Percentage ? percentage : 0;
        Value = discountType == DiscountType.Value ? value : 0;
        MinimumOrderValue = minimumOrderValue;
    }

    public static Discount CreatePercentageDiscount(
        string discountCode, 
        decimal percentage, 
        decimal minimumOrderValue = 0)
    {
        return new Discount(discountCode, DiscountType.Percentage, percentage, 0, minimumOrderValue);
    }

    public static Discount CreateValueDiscount(
        string discountCode, 
        decimal value, 
        decimal minimumOrderValue = 0)
    {
        return new Discount(discountCode, DiscountType.Value, 0, value, minimumOrderValue);
    }

    public decimal Apply(decimal amount)
    {
        if (amount < 0)
            throw new DomainException("Amount cannot be negative.");

        if (amount < MinimumOrderValue)
            return 0;

        if (DiscountType == DiscountType.Percentage && Percentage > 0)
            return (Percentage / 100) * amount;

        if (DiscountType == DiscountType.Value && Value > 0)
            return Value;

        return 0;
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return DiscountCode;
        yield return DiscountType;
        yield return Percentage;
        yield return Value;
        yield return MinimumOrderValue;
    }
}
