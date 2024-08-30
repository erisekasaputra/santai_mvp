using Identity.Contracts.IntegrationEvent;
using MassTransit;

namespace Identity.API.Consumers;

public class StaffUserDeletedIntegrationEventConsumer : IConsumer<StaffUserDeletedIntegrationEvent>
{
    public async Task Consume(ConsumeContext<StaffUserDeletedIntegrationEvent> context)
    {
        throw new NotImplementedException();
    }
}
