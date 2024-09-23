using Core.Enumerations;

namespace Ordering.API.Applications.Dtos.Requests;

public class PaymentRequest
{
    public required Guid OrderId { get; set; }
    public required decimal Amount { get; set; }
    public required Currency Currency { get; set; }
    public required DateTime PaidAt { get; set; }
    public string? PaymentMethod { get; set; }
    public string? BankReference { get; set; }
    public PaymentRequest(
        Guid orderId,
        decimal amount,
        Currency currency,
        DateTime paidAt,
        string? paymentMethod,
        string? bankReference)
    {
        OrderId = orderId;
        Amount = amount;
        Currency = currency;
        PaidAt = paidAt;
        PaymentMethod = paymentMethod;
        BankReference = bankReference;
    }
}
