namespace Chat.API.Applications.Dtos.Request;

public class LatestChatByTimestampRequest
{
    public Guid OrderId { get; set; }
    public long Timestamp { get; set; }
    public bool Forward { get; set; }

    public LatestChatByTimestampRequest()
    {

    }
    public LatestChatByTimestampRequest(
        Guid orderId,
        long timestamp,
        bool forward)
    {
        OrderId = orderId;
        Timestamp = timestamp;
        Forward = forward;
    }
}