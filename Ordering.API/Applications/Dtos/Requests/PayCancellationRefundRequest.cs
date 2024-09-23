using Core.Enumerations;

namespace Ordering.API.Applications.Dtos.Requests;

public class PayCancellationRefundRequest
{
    public required Guid OrderId { get; set; }
    public required decimal Amount { get; set; }
    public required Currency Currency { get; set; }
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
