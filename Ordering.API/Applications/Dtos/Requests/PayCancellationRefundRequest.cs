using Core.Enumerations;

namespace Ordering.API.Applications.Dtos.Requests;

public class PayCancellationRefundRequest
{
    public Guid OrderId { get; set; }
    public decimal Amount { get; set; }
    public Currency Currency { get; set; }
    public PayCancellationRefundRequest(
        Guid orderId,
        decimal amount,
        Currency currency)
    {
        OrderId = orderId;
        Amount = amount;
        Currency = currency;
    }
}
