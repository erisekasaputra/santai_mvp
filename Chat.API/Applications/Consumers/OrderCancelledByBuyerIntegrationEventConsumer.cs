using Chat.API.Applications.Services;
using Chat.API.Applications.Services.Interfaces;
using Core.Events.Ordering;
using MassTransit;
using Microsoft.AspNetCore.SignalR;

namespace Chat.API.Applications.Consumers;

public class OrderCancelledByBuyerIntegrationEventConsumer(IChatService chatService, IHubContext<ChatHub, IChatClient> chatHub) : IConsumer<OrderCancelledByBuyerIntegrationEvent>
{
    private readonly IChatService _chatService = chatService;
    private readonly IHubContext<ChatHub, IChatClient> _chatHub = chatHub;
    public async Task Consume(ConsumeContext<OrderCancelledByBuyerIntegrationEvent> context)
    {
        var chatContact = await _chatService.GetChatContactByOrderId(context.Message.OrderId.ToString());
        if (chatContact == null) 
        {
            return;
        }
         
        await _chatHub.Clients.User(context.Message.BuyerId.ToString()).DeleteChatContact(context.Message.OrderId.ToString());

        if (!string.IsNullOrEmpty(chatContact.MechanicId))
        {
            await _chatHub.Clients.User(chatContact.MechanicId).DeleteChatContact(context.Message.OrderId.ToString());
        }

        await _chatService.DeleteChatContact(context.Message.OrderId.ToString());
         
    }
}
