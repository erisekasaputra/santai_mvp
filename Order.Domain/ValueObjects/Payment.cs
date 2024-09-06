using Order.Domain.Enumerations;
using Order.Domain.SeedWork;

namespace Order.Domain.ValueObjects;

public class Payment : ValueObject
{
    public Money Money { get; set; }
    public DateTime? TransactionAt { get; private set; }
    public string? PaymentMethod { get; private set; }
    public string? BankReference { get; private set; }
    public DateTime CreatedAt { get; private init; }
    public Payment(decimal amount, Currency currency, DateTime transactionAt, string? paymentMethod, string? bankReference)
    {
        Money = new Money(amount, currency);
        TransactionAt = transactionAt;
        PaymentMethod = paymentMethod;
        BankReference = bankReference;
        CreatedAt = DateTime.UtcNow;
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Money;
        yield return TransactionAt;
        yield return PaymentMethod;
        yield return BankReference; 
    }
}
