using Chat.API.Domain.Models; 

namespace Chat.API.Applications.Services.Interfaces;

public interface IChatClient
{
    Task ReceiveMessage(Conversation conversation); 
    Task InternalServerError(string errorMessage);
    Task ChatBadRequest(Guid messageId, Guid orderId);
}
