namespace Chat.API.Applications.Dtos.Request;

public class LatestChatRequest
{
    public Guid OrderId { get; set; }
    public long Timestamp { get; set; }
    public bool Forward { get; set; }

    public LatestChatRequest()
    {
        
    }
    public LatestChatRequest(
        Guid orderId,
        long timestamp,
        bool forward)
    {
        OrderId = orderId;
        Timestamp = timestamp;
        Forward = forward;
    }
}
