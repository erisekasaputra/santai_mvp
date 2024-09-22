using Core.Events.Identity;
using MassTransit;

namespace Notification.Worker.Consumers;

public class AccountSignedInIntegrationEventConsumer : IConsumer<AccountSignedInIntegrationEvent>
{
    public async Task Consume(ConsumeContext<AccountSignedInIntegrationEvent> context)
    {
        throw new NotImplementedException();
    }
}
