using Core.Events;
using MassTransit;

namespace Notification.Worker.Consumers;

public class OrderPaymentPaidIntegrationEventConsumer : IConsumer<OrderPaymentPaidIntegrationEvent>
{
    public async Task Consume(ConsumeContext<OrderPaymentPaidIntegrationEvent> context)
    {
        throw new NotImplementedException();
    }
}
