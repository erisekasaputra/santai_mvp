using Chat.API.Applications.Services.Interfaces;
using Core.Events.Account;
using MassTransit;

namespace Chat.API.Applications.Consumers;

public class AccountMechanicOrderAcceptedIntegrationEventConsumer(
    IChatService chatService) : IConsumer<AccountMechanicOrderAcceptedIntegrationEvent>
{
    private readonly IChatService _chatService = chatService;
    public async Task Consume(ConsumeContext<AccountMechanicOrderAcceptedIntegrationEvent> context)
    {
        var contacts = await _chatService.GetChatContactByOrderId(context.Message.OrderId);

        if (contacts is null)
        {
            return;
        }

        contacts.SetMechanic(context.Message.MechanicId, context.Message.MechanicName);

        await _chatService.UpdateChatContact(contacts);
    }
}
