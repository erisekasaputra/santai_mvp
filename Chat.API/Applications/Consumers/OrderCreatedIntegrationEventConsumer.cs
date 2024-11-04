
using Chat.API.Applications.Services.Interfaces;
using Chat.API.Domain.Models;
using Core.Events.Ordering;
using MassTransit;

namespace Chat.API.Applications.Consumers;

public class OrderCreatedIntegrationEventConsumer(IChatService chatService) : IConsumer<OrderCreatedIntegrationEvent>
{
    private readonly IChatService _chatService = chatService;
    public async Task Consume(ConsumeContext<OrderCreatedIntegrationEvent> context)
    {
        var chatContact = new ChatContact(context.Message.OrderId, context.Message.BuyerId, context.Message.BuyerName);
        await _chatService.CreateChatContact(chatContact);
    }
}
