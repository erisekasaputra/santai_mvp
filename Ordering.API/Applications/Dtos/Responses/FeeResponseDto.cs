using Core.Enumerations; 
using Ordering.Domain.Enumerations;

namespace Ordering.API.Applications.Dtos.Responses;

public class FeeResponseDto
{ 
    public PercentageOrValueType Parameter { get; set; }
    public string FeeDescription { get; set; }
    public Currency Currency { get; set; }
    public decimal ValuePercentage { get; set; }
    public decimal ValueAmount { get; set; }
    public decimal FeeAmount { get; set; }

    public FeeResponseDto(
        PercentageOrValueType parameter,
        string feeDescription,
        Currency currency,
        decimal valuePercentage,
        decimal valueAmount,
        decimal feeAmount)
    {
        Parameter = parameter;
        FeeDescription = feeDescription;
        Currency = currency;
        ValuePercentage = valuePercentage;
        ValueAmount = valueAmount;
        FeeAmount = feeAmount;
    }
}
