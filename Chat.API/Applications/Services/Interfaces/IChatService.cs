using Chat.API.Domain.Models;

namespace Chat.API.Applications.Services.Interfaces;

public interface IChatService
{
    Task<bool> SaveChatMessageAsync(Conversation conversation);
    Task<List<Conversation>?> GetMessageByOrderIdAndTimestamp(Guid orderId, long timestamp, bool forward = true);
    Task<bool> CreateChatContact(ChatContact chatContact);
    Task<List<ChatContact>?> GetChatContactsByBuyerId(Guid buyerId);
    Task<List<ChatContact>?> GetChatContactsByMechanicId(Guid mechanicId);
    Task<ChatContact?> GetChatContactByOrderId(Guid orderId);
    Task<bool> UpdateChatContact(ChatContact chatContact);
    Task DeleteChatContact(Guid orderId);
}
