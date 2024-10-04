using Chat.API.Applications.Services.Interfaces;
using Chat.API.Domain.Events;
using Core.Services.Interfaces;
using MediatR;
using Microsoft.AspNetCore.SignalR;
using static System.Net.Mime.MediaTypeNames;

namespace Chat.API.Applications.Services;

public class ChatHub : Hub<IChatClient>
{
    private readonly IChatService _chatService;
    private readonly IEncryptionService _encryptionService;
    private readonly IMediator _mediator;
    public ChatHub(
        IChatService chatService, 
        IEncryptionService encryptionService,
        IMediator mediator)
    {
        _chatService = chatService;
        _encryptionService = encryptionService;
        _mediator = mediator;
    } 

    public async Task SendMessageToUser(string destinationUserId, string text)
    {
        try
        {
            var originUserId = Context.UserIdentifier;

            if (string.IsNullOrEmpty(originUserId) || string.IsNullOrEmpty(destinationUserId) || string.IsNullOrEmpty(text)) 
            {
                return;
            }
            long timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            var messageId = await _chatService.SaveChatMessageAsync(originUserId, destinationUserId, text, timestamp);
            await Clients.User(destinationUserId).ReceiveChat(originUserId, messageId, text, timestamp.ToString());

            await _mediator.Publish(new ChatSentDomainEvent(Guid.Parse(originUserId), Guid.Parse(destinationUserId), Guid.Parse(messageId), text, timestamp));
        }
        catch (Exception ex)
        { 
            Console.WriteLine($"Error in SendMessageToUser: {ex.Message}"); 
            await Clients.Caller.InternalServerError("Failed to send a message.");
        }
    }

    public async Task GetLatestMessagesForUser(string destinationUserId, long timestamp)
    {
        try
        {  
            var originUserId = Context.UserIdentifier;

            if (string.IsNullOrEmpty(originUserId) || string.IsNullOrEmpty(destinationUserId))
            {
                return;
            }

            var messages = await _chatService.GetMessageByTimestamp(originUserId, destinationUserId, timestamp);

            var tasks = messages.Select(async message =>
            {
                var text = message.GetValueOrDefault("encryptedText")?.S ?? string.Empty;
                var messageId = message.GetValueOrDefault("messageId")?.S ?? string.Empty;
                var timestamp = message.GetValueOrDefault("timestamp")?.N ?? string.Empty;

                if (!string.IsNullOrEmpty(text) && !string.IsNullOrEmpty(messageId) && !string.IsNullOrEmpty(timestamp)) 
                {
                    var decryptedText = await _encryptionService.DecryptAsync(text);
                    await Clients.Caller.ReceiveChat(
                        originUserId,
                        messageId,
                        decryptedText,
                        timestamp);
                } 
            });

            await Task.WhenAll(tasks);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error in RetrieveMessages: {ex.Message}");
            await Clients.Caller.InternalServerError("Failed to retrieve messages.");
        }
    }
     
    public async Task GetPreviousMessagesForUser(string destinationUserId, long timestamp)
    { 
        try
        {
            var originUserId = Context.UserIdentifier;

            if (string.IsNullOrEmpty(originUserId) || string.IsNullOrEmpty(destinationUserId))
            {
                return;
            }

            var messages = await _chatService.GetMessageByTimestamp(originUserId, destinationUserId, timestamp, false);

            var tasks = messages.Select(async message =>
            {
                var text = message.GetValueOrDefault("encryptedText")?.S ?? string.Empty;
                var messageId = message.GetValueOrDefault("messageId")?.S ?? string.Empty;
                var timestamp = message.GetValueOrDefault("timestamp")?.N ?? string.Empty;

                if (!string.IsNullOrEmpty(text) && !string.IsNullOrEmpty(messageId) && !string.IsNullOrEmpty(timestamp))
                {
                    var decryptedText = await _encryptionService.DecryptAsync(text);
                    await Clients.Caller.ReceiveChat(
                        originUserId,
                        messageId,
                        decryptedText,
                        timestamp);
                } 
            });

            await Task.WhenAll(tasks);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error in RetrieveMessages: {ex.Message}");
            await Clients.Caller.InternalServerError("Failed to retrieve messages.");
        }
    }
}
