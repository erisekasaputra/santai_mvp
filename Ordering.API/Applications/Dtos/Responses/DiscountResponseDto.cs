using Core.Enumerations; 
using Ordering.Domain.Enumerations;

namespace Ordering.API.Applications.Dtos.Responses;

public class DiscountResponseDto
{ 
    public string CouponCode { get; private set; }
    public PercentageOrValueType Parameter { get; private set; }
    public Currency Currency { get; private set; }
    public decimal ValuePercentage { get; private set; }
    public decimal ValueAmount { get; private set; }
    public decimal MinimumOrderValue { get; private set; }
    public decimal DiscountAmount { get; private set; }
    public DiscountResponseDto(
        string couponCode,
        PercentageOrValueType parameter,
        Currency currency,
        decimal valuePercentage,
        decimal valueAmount,
        decimal minimumOrderValue,
        decimal discountAmount)
    {
        CouponCode = couponCode;
        Parameter = parameter;
        Currency = currency;
        ValuePercentage = valuePercentage;
        ValueAmount = valueAmount;
        MinimumOrderValue = minimumOrderValue;
        DiscountAmount = discountAmount;
    }
}
