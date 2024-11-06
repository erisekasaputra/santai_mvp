using Chat.API.Applications.Dtos.Response; 

namespace Chat.API.Applications.Services.Interfaces;

public interface IChatClient
{
    Task UpdateChatContact(ChatContactResponse chatContact);
    Task ReceiveChatContact(ChatContactResponse chatContact);
    Task DeleteChatContact(string orderId);
    Task ReceiveMessage(ConversationResponse conversation); 
    Task InternalServerError(string errorMessage);
    Task ChatBadRequest(string messageId, string orderId);
}
