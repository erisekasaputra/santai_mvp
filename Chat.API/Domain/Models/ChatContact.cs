using Amazon.DynamoDBv2.DataModel;

namespace Chat.API.Domain.Models;

[DynamoDBTable("ChatContact")] 
public class ChatContact
{
    [DynamoDBHashKey] 
    public Guid OrderId { get; init; } 
    [DynamoDBRangeKey]  
    public long LastChatTimestamp { get; init; } 
    public Guid BuyerId { get; init; } 
    public string BuyerName { get; init; } 
    public Guid? MechanicId { get; set; } 
    public string? MechanicName { get; set; } 
    public string? LastChatText { get; set; } 
    public DateTime? OrderCompletedAtUtc { get; set; }
    public DateTime? OrderChatExpiredAtUtc { get; set; }
    public bool IsOrderCompleted { get; set; }
    public long ChatUpdateTimestamp { get; set; }

    public ChatContact()
    {
        
    }
    public ChatContact(Guid orderId, Guid buyerId, string buyerName)
    {
        OrderId = orderId;
        BuyerId = buyerId;
        BuyerName = buyerName;
        LastChatTimestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        IsOrderCompleted = false;
    }

    public void UpdateLastChat(string lastChatText)
    {
        if (IsOrderCompleted)
        {
            return;
        }

        LastChatText = lastChatText;
        ChatUpdateTimestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
    }

    public void SetMechanic(Guid mechanicId, string mechanicName)
    {
        if (MechanicId is not null || MechanicId.HasValue || IsOrderCompleted)
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
}
