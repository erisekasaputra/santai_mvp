using Amazon.DynamoDBv2.DataModel;

namespace Chat.API.Domain.Models;

[DynamoDBTable("ChatContact")] 
public class ChatContact
{
    [DynamoDBHashKey] 
    public string OrderId { get; init; } 
    [DynamoDBRangeKey]  
    public long LastChatTimestamp { get; init; } 
    public string BuyerId { get; init; } 
    public string BuyerName { get; init; } 
    public string? MechanicId { get; set; } 
    public string? MechanicName { get; set; } 
    public string? LastChatText { get; set; } 
    public string? ChatOriginUserId { get; set; }
    public DateTime? OrderCompletedAtUtc { get; set; }
    public DateTime? OrderChatExpiredAtUtc { get; set; }
    public bool IsOrderCompleted { get; set; }
    public long ChatUpdateTimestamp { get; set; }
    public bool IsChatExpired { get; set; }


    public ChatContact()
    {
        OrderId = string.Empty;
        BuyerId = string.Empty;
        BuyerName = string.Empty;
    }
    public ChatContact(string orderId, string buyerId, string buyerName)
    {
        OrderId = orderId;
        BuyerId = buyerId;
        BuyerName = buyerName;
        LastChatTimestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        IsOrderCompleted = false;
        IsChatExpired = false;
    }

    public void UpdateLastChat(string originUserId, string lastChatText)
    {
        if (IsOrderCompleted)
        {
            return;
        }

        ChatOriginUserId = originUserId;
        LastChatText = lastChatText;
        ChatUpdateTimestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
    }

    public void SetMechanic(string mechanicId, string mechanicName)
    {
        if (MechanicId is not null || !string.IsNullOrEmpty(MechanicId) || IsOrderCompleted)
        {
            return;
        }

        MechanicId = mechanicId;
        MechanicName = mechanicName;
    }

    public void ResetMechanic()
    {
        MechanicId = null;  
        MechanicName = null;
    }

    public void SetOrderComplete(int totalHoursChatActiveAfterChatComplete = 24)
    {
        if (IsOrderCompleted)
        {
            return;
        }

        OrderCompletedAtUtc = DateTime.UtcNow;
        OrderChatExpiredAtUtc = DateTime.UtcNow.AddHours(totalHoursChatActiveAfterChatComplete);
        IsOrderCompleted = true;
    } 

    public bool IsExpired()
    {
        if (IsOrderCompleted && OrderChatExpiredAtUtc is not null && OrderChatExpiredAtUtc <= DateTime.UtcNow)
        {
            IsChatExpired = true;
            return true;
        }

        IsChatExpired = false;
        return false;
    } 
}
