using static System.Net.Mime.MediaTypeNames;

namespace Chat.API.Applications.Services.Interfaces;

public interface IChatClient
{
    Task ReceiveChat(string originUserId, string messageId, string text, string timestamp);
    Task InternalServerError(string errorMessage);
}
