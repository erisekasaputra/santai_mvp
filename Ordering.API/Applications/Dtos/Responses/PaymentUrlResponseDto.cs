namespace Ordering.API.Applications.Dtos.Responses;

public class PaymentUrlResponseDto
{
    public string PaymentUrl { get; set; }
    public PaymentUrlResponseDto(string paymentUrl)
    {
        PaymentUrl = paymentUrl;
    }
}
