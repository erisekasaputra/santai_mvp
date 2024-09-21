using Ordering.Domain.Enumerations;

namespace Ordering.API.Applications.Dtos.Requests;

public class SenangPayPaymentRequest
{
    public string TransactionId { get; set; }
    public Guid OrderId { get; set; }  
    public decimal Amount { get; set; } 
    public string Method { get; set; }
    public string Message { get; set; }
    public int Status { get; set; }
    public string Hash { get; set; } 
    public SenangPayPaymentRequest(
        string transactionId,
        Guid orderId,  
        decimal amount,
        string method,
        string message,
        int status,
        string hash)
    {
        OrderId = orderId;  
        Method = method;
        TransactionId = transactionId;
        Message = message;
        Status = status;
        Hash = hash;
    }
}
