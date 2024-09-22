using Core.Events.Identity;
using MassTransit;

namespace Notification.Worker.Consumers;

public class AccountSignedOutIntegrationEventConsumer : IConsumer<AccountSignedOutIntegrationEvent>
{
    public async Task Consume(ConsumeContext<AccountSignedOutIntegrationEvent> context)
    {
        throw new NotImplementedException();
    }
}
