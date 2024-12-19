namespace Ordering.API.Applications.Dtos.Responses;

public class BuyerResponseDto
{
    public Guid BuyerId { get; set; }
    public string BuyerName { get; set; }
    public string BuyerImageUrl { get; set; }
    public string? Email { get; set; }
    public string? PhoneNumber { get; set; }
    public BuyerResponseDto(
        Guid buyerId,
        string buyerName,
        string buyerImageUrl,
        string? email,
        string? phoneNumber)
    {
        BuyerId = buyerId;
        BuyerName = buyerName;
        BuyerImageUrl = buyerImageUrl;
        Email = email;
        PhoneNumber = phoneNumber;
    }
}
