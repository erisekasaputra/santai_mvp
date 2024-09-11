using Core.Enumerations;

namespace Ordering.API.Applications.Dtos.Responses;

public class TaxResponseDto
{
    public decimal Rate { get; private set; }
    public Currency Currency { get; private set; }
    public decimal Amount { get; private set; }
    public TaxResponseDto(
        decimal rate, 
        Currency currency, 
        decimal amount)
    {
        Rate = rate;
        Currency = currency;
        Amount = amount;
    }
}
