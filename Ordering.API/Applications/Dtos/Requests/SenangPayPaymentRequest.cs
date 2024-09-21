using Ordering.Domain.Enumerations;

namespace Ordering.API.Applications.Dtos.Requests;

public class SenangPayPaymentRequest
{
    public Guid OrderId { get; set; } 
    public string Name { get; set; }
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public decimal Amount { get; set; } 
    public string Method { get; set; }
    public string Reference { get; set; }
    public string Message { get; set; }
    public string Hash { get; set; } 
    public int Status { get; set; }
    public SenangPayPaymentRequest(
        Guid orderId, 
        string name,
        string? email,
        string? phone,
        decimal amount,
        string method,
        string reference,
        string message,
        string hash,
        int status)
    {
        OrderId = orderId; 
        Name = name;
        Email = email;
        Phone = phone;
        Amount = amount;
        Method = method;
        Reference = reference;
        Message = message;
        Hash = hash;
        Status = status;
    }
}
