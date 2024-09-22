using Core.Events.Identity;
using MassTransit;

namespace Notification.Worker.Consumers;

public class AccountSignedOutIntegrationEventConsumer : IConsumer<AccountSignedOutIntegrationEvent>
{
    public async Task Consume(ConsumeContext<AccountSignedOutIntegrationEvent> context)
    {
        Console.WriteLine(context.Message.UserId);
        Console.WriteLine(context.Message.DeviceId);
    }
}
