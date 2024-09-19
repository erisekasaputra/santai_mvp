using Core.Events;
using MassTransit;

namespace Notification.Worker.Consumers;
public class AccountMechanicOrderAcceptedIntegrationEventConsumer : IConsumer<AccountMechanicOrderAcceptedIntegrationEvent>
{
    public async Task Consume(ConsumeContext<AccountMechanicOrderAcceptedIntegrationEvent> context)
    {
        throw new NotImplementedException();
    }
}
