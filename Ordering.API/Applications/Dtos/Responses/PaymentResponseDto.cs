using Core.Enumerations; 

namespace Ordering.API.Applications.Dtos.Responses;

public class PaymentResponseDto
{ 
    public Currency Currency { get; set; }
    public decimal Amount { get; set; }
    public DateTime? PaidAt { get; private set; }
    public string? PaymentMethod { get; private set; }
    public string? BankReference { get; private set; } 
}
