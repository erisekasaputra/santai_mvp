using Amazon.DynamoDBv2.Model;

namespace Chat.API.Applications.Services.Interfaces;

public interface IChatService
{
    Task<string> SaveChatMessageAsync(string originUserId, string destinationUserId, string text, long timestamp);
    Task<List<Dictionary<string, AttributeValue>>> GetMessageByTimestamp(string originUserId, string destinationUserId, long timestamp, bool forward = true);
}
