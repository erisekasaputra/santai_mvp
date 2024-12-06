using Chat.API.Applications.Dtos.Request;
using Chat.API.Applications.Mapper;
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

    public async Task SendMessage(SendMessageRequest request)
    { 
        try
        { 
            var originUserId = Context.UserIdentifier; 
            if (string.IsNullOrEmpty(originUserId) || string.IsNullOrEmpty(request.DestinationUserId) || string.IsNullOrEmpty(request.Text) || string.IsNullOrEmpty(request.OrderId)) 
            {
                await Clients.Caller.ChatBadRequest("Missing required fields.", string.Empty);
                return;
            }

            var chatContact = await _chatService.GetChatContactByOrderId(request.OrderId);

            if (chatContact is null)
            {
                await Clients.Caller.ChatBadRequest("Chat session is no longer available", request.OrderId);
                return;
            } 

            if (chatContact.IsExpired())
            {
                await _chatService.UpdateChatContact(chatContact);
                await Clients.User(chatContact.BuyerId).UpdateChatContact(chatContact.ToResponse());
                if (chatContact.MechanicId is null)
                {
                    return;
                }
                await Clients.User(chatContact.MechanicId).UpdateChatContact(chatContact.ToResponse());
                return;
            }

            if (request.DestinationUserId != chatContact.BuyerId && request.DestinationUserId != chatContact.MechanicId)
            {
                await Clients.Caller.ChatBadRequest("Chat session is no longer available", chatContact.OrderId);
                return;
            }
             
            if (string.IsNullOrEmpty(chatContact.MechanicId))
            { 
                await Clients.Caller.ChatBadRequest("Waiting for mechanic assignment", chatContact.OrderId);
                return;
            }

            var conversation = new Conversation(
                request.OrderId,
                originUserId,
                request.DestinationUserId,
                request.Text,
                request.Attachment,
                request.ReplyMessageId,
                request.ReplyMessageText);
             
            chatContact.UpdateLastChat(conversation.OriginUserId, conversation.Text);

            _ = await _chatService.SaveChatMessageAsync(conversation);
            _ = await _chatService.UpdateChatContact(chatContact);

            await Clients.Caller.ReceiveMessage(conversation.ToResponse());
            await Clients.User(request.DestinationUserId).ReceiveMessage(conversation.ToResponse());

            await Clients.Caller.UpdateChatContact(chatContact.ToResponse());
            await Clients.User(request.DestinationUserId).UpdateChatContact(chatContact.ToResponse()); 

            await _mediator.Publish(new ChatSentDomainEvent(conversation));
        } 
        catch (Exception ex)
        { 
            LoggerHelper.LogError(_logger, ex);
            await Clients.Caller.InternalServerError($"An internal server error has occured, Error: {ex.Message}, Error Detail: {ex.InnerException?.Message}");
        }
    } 
}
