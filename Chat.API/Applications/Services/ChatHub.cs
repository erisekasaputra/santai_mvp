using Chat.API.Applications.Services.Interfaces;
using Chat.API.Domain.Events;
using Core.Services.Interfaces;
using Core.Utilities;
using MediatR;
using Microsoft.AspNetCore.SignalR; 

namespace Chat.API.Applications.Services;

public class ChatHub(
    IChatService chatService,
    IEncryptionService encryptionService,
    IMediator mediator,
    ILogger<ChatHub> logger) : Hub<IChatClient>
{
    private readonly IChatService _chatService = chatService;
    private readonly IEncryptionService _encryptionService = encryptionService;
    private readonly IMediator _mediator = mediator;
    private readonly ILogger<ChatHub> _logger = logger;

    public async Task SendMessageToUser(
        string destinationUserId, 
        string text,
        string? replyMessageId,
        string? replyMessageText)
    {
        try
        {
            var originUserId = Context.UserIdentifier;

            Console.WriteLine("Step 1 completed origin: {0}, destination: {1}", originUserId, destinationUserId);

            if (string.IsNullOrEmpty(originUserId) || string.IsNullOrEmpty(destinationUserId) || string.IsNullOrEmpty(text)) 
            {
                return;
            }
             
            Console.WriteLine("Step 2 completed origin: {0}, destination: {1}", originUserId, destinationUserId);

            long timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

            var messageId = await _chatService.SaveChatMessageAsync(
                originUserId, 
                destinationUserId, 
                text, 
                replyMessageId,
                replyMessageText,
                timestamp);

            await Clients.User(destinationUserId).ReceiveChat(
                originUserId,
                messageId,
                text,
                string.IsNullOrEmpty(replyMessageId) ? string.Empty : replyMessageId,
                string.IsNullOrEmpty(replyMessageText) ? string.Empty : replyMessageText,
                timestamp.ToString());

            await _mediator.Publish(new ChatSentDomainEvent(Guid.Parse(originUserId), Guid.Parse(destinationUserId), Guid.Parse(messageId), text, timestamp));
        }
        catch (Exception ex)
        { 
            LoggerHelper.LogError(_logger, ex);
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
                var replyMessageId = message.GetValueOrDefault("replyMessageId")?.S ?? string.Empty;
                var replyMessageEncryptedText = message.GetValueOrDefault("replyMessageEncryptedText")?.S ?? string.Empty; 
                var timestamp = message.GetValueOrDefault("timestamp")?.N ?? string.Empty;

                if (!string.IsNullOrEmpty(text) && !string.IsNullOrEmpty(messageId) && !string.IsNullOrEmpty(timestamp)) 
                {
                    var decryptedText = await _encryptionService.DecryptAsync(text);
                    var decryptedReplyMessageText = string.IsNullOrEmpty(replyMessageEncryptedText) ? string.Empty : await _encryptionService.DecryptAsync(replyMessageEncryptedText);

                    await Clients.Caller.ReceiveChat(
                        originUserId,
                        messageId,
                        decryptedText,
                        replyMessageId,
                        decryptedReplyMessageText,
                        timestamp);
                } 
            });

            await Task.WhenAll(tasks);
        }
        catch (Exception ex)
        {
            LoggerHelper.LogError(_logger, ex);
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
                var replyMessageId = message.GetValueOrDefault("replyMessageId")?.S ?? string.Empty;
                var replyMessageEncryptedText = message.GetValueOrDefault("replyMessageEncryptedText")?.S ?? string.Empty;
                var timestamp = message.GetValueOrDefault("timestamp")?.N ?? string.Empty;

                if (!string.IsNullOrEmpty(text) && !string.IsNullOrEmpty(messageId) && !string.IsNullOrEmpty(timestamp))
                {
                    var decryptedText = await _encryptionService.DecryptAsync(text);
                    var decryptedReplyMessageText = string.IsNullOrEmpty(replyMessageEncryptedText) ? string.Empty : await _encryptionService.DecryptAsync(replyMessageEncryptedText);

                    await Clients.Caller.ReceiveChat(
                         originUserId,
                         messageId,
                         decryptedText,
                         replyMessageId,
                         decryptedReplyMessageText,
                         timestamp);
                } 
            });

            await Task.WhenAll(tasks);
        }
        catch (Exception ex)
        {
            LoggerHelper.LogError(_logger, ex);
            await Clients.Caller.InternalServerError("Failed to retrieve messages."); 
        }
    }
}
