
using Chat.API.Applications.Mapper;
using Chat.API.Applications.Services;
using Chat.API.Applications.Services.Interfaces;
using Chat.API.Domain.Models;
using Core.Events.Ordering;
using MassTransit;
using Microsoft.AspNetCore.SignalR;

namespace Chat.API.Applications.Consumers;

public class OrderCreatedIntegrationEventConsumer(IChatService chatService, IHubContext<ChatHub, IChatClient> chatHub) : IConsumer<OrderCreatedIntegrationEvent>
{
    private readonly IChatService _chatService = chatService;
    private readonly IHubContext<ChatHub, IChatClient> _chatHub = chatHub;
    public async Task Consume(ConsumeContext<OrderCreatedIntegrationEvent> context)
    {  
        var chatContact = new ChatContact(context.Message.OrderId.ToString(), context.Message.BuyerId.ToString(), context.Message.BuyerName, context.Message.BuyerImageUrl);

        await _chatService.CreateChatContact(chatContact);

        await _chatHub.Clients.User(context.Message.BuyerId.ToString()).ReceiveChatContact(chatContact.ToResponse());
    }
}
