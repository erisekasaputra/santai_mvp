using Chat.API.Applications.Mapper;
using Chat.API.Applications.Services;
using Chat.API.Applications.Services.Interfaces;
using Core.Events.Account;
using MassTransit;
using Microsoft.AspNetCore.SignalR;

namespace Chat.API.Applications.Consumers;

public class AccountMechanicOrderAcceptedIntegrationEventConsumer(
    IChatService chatService, IHubContext<ChatHub, IChatClient> chatHub) : IConsumer<AccountMechanicOrderAcceptedIntegrationEvent>
{
    private readonly IChatService _chatService = chatService;
    private readonly IHubContext<ChatHub, IChatClient> _chatHub = chatHub;
    public async Task Consume(ConsumeContext<AccountMechanicOrderAcceptedIntegrationEvent> context)
    {
        var contact = await _chatService.GetChatContactByOrderId(context.Message.OrderId.ToString());

        if (contact is null)
        {
            return;
        }

        contact.SetMechanic(context.Message.MechanicId.ToString(), context.Message.MechanicName);

        await _chatService.UpdateChatContact(contact);
        
        await _chatHub.Clients.User(context.Message.BuyerId.ToString()).UpdateChatContact(contact.ToResponse());
        await _chatHub.Clients.User(context.Message.MechanicId.ToString()).UpdateChatContact(contact.ToResponse());
    }
}
