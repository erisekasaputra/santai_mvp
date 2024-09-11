namespace Ordering.API.Applications.Dtos.Responses;

public class BuyerResponseDto
{
    public Guid BuyerId { get; private set; }
    public string BuyerName { get; private set; }
    public BuyerResponseDto(
        Guid buyerId,
        string buyerName)
    {
        BuyerId = buyerId;
        BuyerName = buyerName;
    }
}
