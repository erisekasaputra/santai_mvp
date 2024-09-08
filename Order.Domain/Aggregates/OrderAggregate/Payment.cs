using Order.Domain.Enumerations;
using Order.Domain.SeedWork;
using Order.Domain.ValueObjects;

namespace Order.Domain.Aggregates.OrderAggregate;

public class Payment : Entity
{
    public Guid OrderingId { get; private set; }
    public Ordering Ordering { get; private set; }
    public Money Amount { get; set; }
    public DateTime? TransactionAt { get; private set; }
    public string? PaymentMethod { get; private set; }
    public string? BankReference { get; private set; }
    public DateTime CreatedAt { get; private init; }
    public Payment()
    {
        Ordering = null!;
        Amount = null!;
    }
    public Payment(Guid orderingId, decimal amount, Currency currency, DateTime transactionAt, string? paymentMethod, string? bankReference)
    {
        OrderingId = orderingId;
        Amount = new Money(amount, currency);
        TransactionAt = transactionAt;
        PaymentMethod = paymentMethod;
        BankReference = bankReference;
        CreatedAt = DateTime.UtcNow;
        Ordering = null!;
    }
}

