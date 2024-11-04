using Chat.API.Applications.Services.Interfaces;
using Core.Events.Ordering;
using MassTransit;

namespace Chat.API.Applications.Consumers;

public class OrderCancelledByMechanicIntegrationEventConsumer
    (IChatService chatService) : IConsumer<OrderCancelledByMechanicIntegrationEvent>
{
    private readonly IChatService _chatService = chatService;
    public async Task Consume(ConsumeContext<OrderCancelledByMechanicIntegrationEvent> context)
    {
        var contacts = await _chatService.GetChatContactByOrderId(context.Message.OrderId);

        if (contacts is null)
        {
            return;
        }

        contacts.ResetMechanic();

        await _chatService.UpdateChatContact(contacts);
    }
}
