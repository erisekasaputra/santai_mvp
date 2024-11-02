namespace Chat.API.Domain.Models;

public class ChatContact
{
    public Guid OrderId { get; set; }
    public Guid BuyerId { get; set; }
    public string BuyerName { get; set; }
    public Guid? MechanicId { get; set; }
    public string? MechanicName { get; set; }
    public string? LastChatText { get; set; }
    public long LastChatTimestamp { get; set; }
    public DateTime? OrderCompletedAtUtc { get; set; }
    public DateTime? OrderChatExpiredAtUtc { get; set; }
    public bool IsOrderCompleted { get; set; }

    public ChatContact(Guid orderId, Guid buyerId, string buyerName, long lastChatTimestamp)
    {
        OrderId = orderId;
        BuyerId = buyerId;
        BuyerName = buyerName;
        LastChatTimestamp = lastChatTimestamp;
        IsOrderCompleted = false;
    }

    public void UpdateLastChat(string lastChatText, long lastChatTimestamp)
    {
        if (IsOrderCompleted)
        {
            return;
        }

        LastChatText = lastChatText;
        LastChatTimestamp = lastChatTimestamp;
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
