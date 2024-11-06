using Chat.API.Applications.Services;
using Chat.API.Applications.Services.Interfaces;
using Core.Events.Ordering;
using MassTransit;
using Microsoft.AspNetCore.SignalR;

namespace Chat.API.Applications.Consumers;

public class ServiceCompletedIntegrationEventConsumer(
    IChatService chatService, IHubContext<ChatHub, IChatClient> chatHub) : IConsumer<ServiceCompletedIntegrationEvent>
{
    private readonly IChatService _chatService = chatService;
    private readonly IHubContext<ChatHub, IChatClient> _chatHub = chatHub;
    public async Task Consume(ConsumeContext<ServiceCompletedIntegrationEvent> context)
    {
        var chatContact = await _chatService.GetChatContactByOrderId(context.Message.OrderId.ToString());
        if (chatContact is null)
        {
            return;
        }

        chatContact.SetOrderComplete(24 * 7);
        await _chatService.UpdateChatContact(chatContact);
    }
}
