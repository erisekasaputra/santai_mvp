using Chat.API.Applications.Dtos.Response;
using Chat.API.Domain.Models;

namespace Chat.API.Applications.Mapper;

public static class ChatContactMapper
{
    public static ChatContactResponse ToResponse(this ChatContact contact)
    {
        return new ChatContactResponse(
            contact.OrderId,
            contact.LastChatTimestamp,
            contact.BuyerId,
            contact.BuyerName,
            contact.MechanicImageUrl ?? "",
            contact.MechanicId ?? string.Empty,
            contact.MechanicName ?? string.Empty,
            contact.LastChatText ?? string.Empty,
            contact.ChatOriginUserId ?? string.Empty,
            (contact.OrderCompletedAtUtc is null ? string.Empty : contact.OrderCompletedAtUtc?.ToString("o")) ?? string.Empty ,
            (contact.OrderChatExpiredAtUtc is null ? string.Empty : contact.OrderChatExpiredAtUtc?.ToString("o")) ?? string.Empty,
            contact.IsOrderCompleted,
            contact.ChatUpdateTimestamp,
            contact.IsChatExpired);
    }
}
