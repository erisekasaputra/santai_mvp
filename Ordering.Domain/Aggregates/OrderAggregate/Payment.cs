using Core.Enumerations;
using Core.Exceptions;
using Core.ValueObjects; 
using Ordering.Domain.SeedWork;

namespace Ordering.Domain.Aggregates.OrderAggregate;

public class Payment : Entity
{
    public Guid OrderId { get; private set; }
    public Order Order { get; private set; }
    public Money Amount { get; set; }
    public DateTime? TransactionAt { get; private set; }
    public string? PaymentMethod { get; private set; }
    public string? BankReference { get; private set; }
    public DateTime CreatedAt { get; private init; }

    public Payment()
    {
        Order = null!;
        Amount = null!;
    } 
    public Payment(
        Guid orderId,
        decimal amount,
        Currency currency,
        DateTime transactionAt,
        string? paymentMethod,
        string? bankReference)
    {
        OrderId = orderId;
        Amount = new Money(amount, currency);
        TransactionAt = transactionAt;
        PaymentMethod = paymentMethod;
        BankReference = bankReference;
        CreatedAt = DateTime.UtcNow;
        Order = null!;
    }

    public void SetPayment(decimal amount, Currency currency)
    {
        if (currency != Amount.Currency)
        {
            throw new DomainException("Payment currency is not equal with payment aggregate currency");
        }

        Amount.SetAmount(amount, currency);
    }
}

