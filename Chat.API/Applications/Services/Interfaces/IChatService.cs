using Chat.API.Domain.Models;

namespace Chat.API.Applications.Services.Interfaces;

public interface IChatService
{
    Task<bool> SaveChatMessageAsync(Conversation conversation);
    IAsyncEnumerable<Conversation> GetMessageByOrderId(string orderId, bool forward = true);
    Task<bool> CreateChatContact(ChatContact chatContact);
    Task<List<ChatContact>?> GetChatContactsByBuyerId(string buyerId);
    Task<List<ChatContact>?> GetChatContactsByMechanicId(string mechanicId);
    Task<ChatContact?> GetChatContactByOrderId(string orderId);
    Task<bool> UpdateChatContact(ChatContact chatContact);
    Task DeleteChatContact(string orderId, bool isDeletingConversations = true);
    IAsyncEnumerable<Conversation> GetMessageByLastTimestamp(
        string orderId,
        long timestamp,
        bool forward = true);
}
