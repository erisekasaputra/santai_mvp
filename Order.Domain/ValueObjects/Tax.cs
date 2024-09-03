using Order.Domain.Exceptions;
using Order.Domain.SeedWork;

namespace Order.Domain.ValueObjects;

public class Tax : ValueObject
{
    public decimal Rate { get; private set; } 
    public Tax(decimal rate)
    {
        if (rate < 0)
            throw new DomainException("Tax rate cannot be negative.");

        Rate = rate;
    }

    public decimal Apply(decimal amount)
    {
        if (amount < 0)
            throw new DomainException("Amount cannot be negative.");

        return (Rate / 100) * amount;
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Rate;
    }
}
