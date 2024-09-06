using Order.Domain.Exceptions;
using Order.Domain.SeedWork;

namespace Order.Domain.ValueObjects;

public class Tax : ValueObject
{
    public decimal Rate { get; private set; }   

    public Tax(decimal rate)
    {
        if (Rate < 1 || Rate > 100)
            throw new DomainException("Discount percentage must be between 1 and 100.");

        Rate = rate; 
    }

    public Money Apply(Money subtotal)
    {
        if (subtotal.Amount < 0)
            throw new DomainException("Amount cannot be negative.");

        var taxAmount = new Money(subtotal.Amount * (Rate / 100), subtotal.Currency); 
        
        return taxAmount;
    } 

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Rate; 
    }
}
