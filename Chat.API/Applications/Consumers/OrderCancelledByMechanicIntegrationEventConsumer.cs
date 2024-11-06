using Chat.API.Applications.Mapper;
using Chat.API.Applications.Services;
using Chat.API.Applications.Services.Interfaces;
using Core.Events.Ordering;
using MassTransit;
using Microsoft.AspNetCore.SignalR;

namespace Chat.API.Applications.Consumers;

public class OrderCancelledByMechanicIntegrationEventConsumer
    (IChatService chatService, IHubContext<ChatHub, IChatClient> chatHub) : IConsumer<OrderCancelledByMechanicIntegrationEvent>
{
    private readonly IChatService _chatService = chatService;
    private readonly IHubContext<ChatHub, IChatClient> _chatHub = chatHub;
    public async Task Consume(ConsumeContext<OrderCancelledByMechanicIntegrationEvent> context)
    {
        var contact = await _chatService.GetChatContactByOrderId(context.Message.OrderId.ToString());
       
        if (contact is null)
        {
            return;
        }

        var oldMechanicId = contact.MechanicId;

        contact.ResetMechanic(); 
        await _chatService.UpdateChatContact(contact);

        await _chatHub.Clients.User(contact.BuyerId).UpdateChatContact(contact.ToResponse());

        if (oldMechanicId is not null)
        {
            await _chatHub.Clients.User(oldMechanicId).UpdateChatContact(contact.ToResponse());
        }
    }
}
