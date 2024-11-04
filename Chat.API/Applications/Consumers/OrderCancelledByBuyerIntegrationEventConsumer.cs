using Chat.API.Applications.Services.Interfaces;
using Core.Events.Ordering;
using MassTransit;

namespace Chat.API.Applications.Consumers;

public class OrderCancelledByBuyerIntegrationEventConsumer(IChatService chatService) : IConsumer<OrderCancelledByBuyerIntegrationEvent>
{
    private readonly IChatService _chatService = chatService;
    public async Task Consume(ConsumeContext<OrderCancelledByBuyerIntegrationEvent> context)
    {
        await _chatService.DeleteChatContact(context.Message.OrderId);
    }
}
