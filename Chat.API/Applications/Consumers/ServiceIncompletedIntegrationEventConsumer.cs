using Chat.API.Applications.Services.Interfaces;
using Core.Events.Ordering;
using MassTransit;

namespace Chat.API.Applications.Consumers;

public class ServiceIncompletedIntegrationEventConsumer(
    IChatService chatService) : IConsumer<ServiceIncompletedIntegrationEvent>
{
    private readonly IChatService _chatService = chatService;
    public async Task Consume(ConsumeContext<ServiceIncompletedIntegrationEvent> context)
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
