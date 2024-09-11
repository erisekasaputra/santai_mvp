using Core.Enumerations;
using Core.Exceptions;
using Core.ValueObjects;
using Ordering.Domain.SeedWork;

namespace Ordering.Domain.ValueObjects;

public class Tax : ValueObject
{
    public decimal Rate { get; private set; }
    public Money TaxAmount { get; private set; }
    public Tax()
    {
        Rate = 0;
        TaxAmount = null!;
    }
    public Tax(decimal rate, Currency currency)
    {
        if (rate < 1 || rate > 100)
            throw new DomainException("Tax percentage must be between 1 and 100.");

        Rate = rate;
        TaxAmount = new Money(0, currency);
    }

    public Money Apply(Money subtotal)
    {
        if (subtotal.Amount < 0)
            throw new DomainException("Amount cannot be negative.");

        if (subtotal.Currency != TaxAmount.Currency)
        {
            throw new DomainException("Tax currency should be the same as subtotal currency");
        }

        var taxAmount = new Money(subtotal.Amount * (Rate / 100), subtotal.Currency);

        TaxAmount = taxAmount;

        return taxAmount;
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Rate;
    }
}
