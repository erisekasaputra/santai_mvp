using Order.Domain.Enumerations;
using Order.Domain.Exceptions;

namespace Order.Domain.ValueObjects;

public class Money : IEquatable<Money>
{
    public decimal Amount { get; private set; }
    public Currency Currency { get; private set; }

    public Money(decimal amount, Currency currency)
    {
        if (amount < 0)
            throw new DomainException("Money amount cannot be negative.");

        Amount = amount;
        Currency = currency;
    }

    public Money ConvertCurrency(Currency targetCurrency, Func<Currency, Currency, decimal> exchangeRate)
    {
        if (Currency == targetCurrency)
            return this;

        var rate = exchangeRate(Currency, targetCurrency);
        return new Money(Amount * rate, targetCurrency);
    }

    public static Money operator +(Money a, Money b)
    {
        if (a.Currency != b.Currency)
            throw new DomainException("Cannot add two Money values with different currencies.");

        return new Money(a.Amount + b.Amount, a.Currency);
    }

    public static Money operator -(Money a, Money b)
    {
        if (a.Currency != b.Currency)
            throw new DomainException("Cannot subtract two Money values with different currencies.");

        return new Money(a.Amount - b.Amount, a.Currency);
    }

    public static bool operator <(Money a, Money b)
    {
        if (a.Currency != b.Currency)
            throw new DomainException("Cannot subtract two Money values with different currencies.");

        return a.Amount < b.Amount; 
    }

    public static bool operator >(Money a, Money b)
    {
        if (a.Currency != b.Currency)
            throw new DomainException("Cannot subtract two Money values with different currencies.");

        return a.Amount > b.Amount;
    }
    public static bool operator <=(Money a, Money b)
    {
        if (a.Currency != b.Currency)
            throw new DomainException("Cannot compare two Money values with different currencies.");

        return a.Amount <= b.Amount;
    }

    public static bool operator >=(Money a, Money b)
    {
        if (a.Currency != b.Currency)
            throw new DomainException("Cannot compare two Money values with different currencies.");

        return a.Amount >= b.Amount;
    }

    public bool Equals(Money? other) => other != null && Amount == other.Amount && Currency == other.Currency;

    public override bool Equals(object? obj) => obj is Money money && Equals(money);

    public override int GetHashCode() => HashCode.Combine(Amount, Currency);
}