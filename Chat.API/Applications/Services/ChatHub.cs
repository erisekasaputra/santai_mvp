using Chat.API.Applications.Dtos.Request;
using Chat.API.Applications.Services.Interfaces;
using Chat.API.Domain.Events;
using Chat.API.Domain.Models; 
using Core.Utilities;
using MediatR;
using Microsoft.AspNetCore.SignalR; 

namespace Chat.API.Applications.Services;

public class ChatHub(
    IChatService chatService, 
    IMediator mediator,
    ILogger<ChatHub> logger) : Hub<IChatClient>
{
    private readonly IChatService _chatService = chatService; 
    private readonly IMediator _mediator = mediator;
    private readonly ILogger<ChatHub> _logger = logger;

    public override Task OnConnectedAsync()
    { 
        if (Context?.User?.Identity == null || !Context.User.Identity.IsAuthenticated)
        { 
            Context?.Abort(); 
            return Task.CompletedTask;  
        }

        return base.OnConnectedAsync();
    }

    public override Task OnDisconnectedAsync(Exception? exception)
    { 
        return base.OnDisconnectedAsync(exception);
    }

    public async Task SendMessageToUser(SendMessageRequest request)
    {
        Guid messageId = Guid.Empty; 
        try
        { 
            var originUserId = Context.UserIdentifier; 
            if (string.IsNullOrEmpty(originUserId) || string.IsNullOrEmpty(request.DestinationUserId) || string.IsNullOrEmpty(request.Text) || string.IsNullOrEmpty(request.OrderId)) 
            {
                return;
            }


            var conversation = new Conversation(
                Guid.Parse(request.OrderId),
                Guid.Parse(originUserId),
                Guid.Parse(request.DestinationUserId),
                request.Text,
                request.Attachment,
                string.IsNullOrEmpty(request.ReplyMessageId) ? null : Guid.Parse(request.ReplyMessageId),
                request.ReplyMessageText);

            messageId = conversation.MessageId; 

            _ = await _chatService.SaveChatMessageAsync(conversation);

            await Clients.User(request.DestinationUserId).ReceiveChat(conversation);

            await _mediator.Publish(new ChatSentDomainEvent(conversation));
        }
        catch (InvalidOperationException ex)
        {
            LoggerHelper.LogError(_logger, ex);
            await Clients.Caller.ChatBadRequest(messageId, Guid.Parse(request.OrderId));
        }
        catch (Exception ex)
        { 
            LoggerHelper.LogError(_logger, ex);
            await Clients.Caller.InternalServerError($"An internal server error has occured, Error: {ex.Message}, Error Detail: {ex.InnerException?.Message}");
        }
    } 
}
