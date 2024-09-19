using Core.Events;
using MassTransit;

namespace Notification.Worker.Consumers;
public class RefundPaidIntegrationEventConsumer : IConsumer<RefundPaidIntegrationEvent>
{
    public async Task Consume(ConsumeContext<RefundPaidIntegrationEvent> context)
    {
        throw new NotImplementedException();
    }
}
