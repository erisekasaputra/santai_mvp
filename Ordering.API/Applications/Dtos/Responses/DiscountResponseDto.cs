using Core.Enumerations; 
using Ordering.Domain.Enumerations;

namespace Ordering.API.Applications.Dtos.Responses;

public class DiscountResponseDto
{ 
    public string CouponCode { get; set; }
    public PercentageOrValueType Parameter { get; set; }
    public Currency Currency { get; set; }
    public decimal ValuePercentage { get; set; }
    public decimal ValueAmount { get; set; }
    public decimal MinimumOrderValue { get; set; }
    public decimal DiscountAmount { get; set; }
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
