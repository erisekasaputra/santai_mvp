using Core.Enumerations; 
using Ordering.Domain.Enumerations;

namespace Ordering.API.Applications.Dtos.Responses;

public class FeeResponseDto
{ 
    public PercentageOrValueType Parameter { get; private set; }
    public FeeDescription FeeDescription { get; private set; }
    public Currency Currency { get; private set; }
    public decimal ValuePercentage { get; private set; }
    public decimal ValueAmount { get; private set; }
    public decimal FeeAmount { get; private set; }

    public FeeResponseDto(
        PercentageOrValueType parameter,
        FeeDescription feeDescription,
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
