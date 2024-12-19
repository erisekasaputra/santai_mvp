 
namespace Chat.API.Applications.Dtos.Response;

public class ChatContactResponse
{ 
    public string OrderId { get; init; } 
    public long LastChatTimestamp { get; init; }
    public string BuyerId { get; init; }
    public string BuyerName { get; init; }
    public string MechanicId { get; set; }
    public string MechanicName { get; set; }
    public string MechanicImageUrl { get; set; }
    public string LastChatText { get; set; }
    public string ChatOriginUserId { get; set; }
    public string OrderCompletedAtUtc { get; set; }
    public string OrderChatExpiredAtUtc { get; set; }
    public bool IsOrderCompleted { get; set; }
    public long ChatUpdateTimestamp { get; set; }
    public bool IsChatExpired { get; set; }
    public ChatContactResponse(
       string orderId,
       long lastChatTimestamp,
       string buyerId,
       string buyerName,
       string mechanicId,
       string mechanicName,
       string mechanicImageUrl,
       string lastChatText,
       string chatOriginUserId,
       string orderCompletedAtUtc,
       string orderChatExpiredAtUtc,
       bool isOrderCompleted,
       long chatUpdateTimestamp,
       bool isChatExpired)
    {
        OrderId = orderId;
        LastChatTimestamp = lastChatTimestamp;
        BuyerId = buyerId;
        BuyerName = buyerName;
        MechanicId = mechanicId;
        MechanicName = mechanicName;
        MechanicImageUrl = mechanicImageUrl;
        LastChatText = lastChatText;
        ChatOriginUserId = chatOriginUserId;
        OrderCompletedAtUtc = orderCompletedAtUtc;
        OrderChatExpiredAtUtc = orderChatExpiredAtUtc;
        IsOrderCompleted = isOrderCompleted;
        ChatUpdateTimestamp = chatUpdateTimestamp;
        IsChatExpired = isChatExpired;
    }

}
