using Core.Events.Identity;
using MassTransit;

namespace Notification.Worker.Consumers;

public class AccountSignedInIntegrationEventConsumer : IConsumer<AccountSignedInIntegrationEvent>
{
    public async Task Consume(ConsumeContext<AccountSignedInIntegrationEvent> context)
    {
        Console.WriteLine(context.Message.UserId);
        Console.WriteLine(context.Message.DeviceId);
    }
}
