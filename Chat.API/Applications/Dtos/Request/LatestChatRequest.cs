namespace Chat.API.Applications.Dtos.Request;

public class LatestChatRequest
{
    public Guid OrderId { get; set; } 
    public bool Forward { get; set; }

    public LatestChatRequest()
    {
        
    }
    public LatestChatRequest(
        Guid orderId, 
        bool forward)
    {
        OrderId = orderId; 
        Forward = forward;
    }
}
