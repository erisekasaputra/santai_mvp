using Identity.Contracts.IntegrationEvent;
using MassTransit;

namespace Account.API.Consumers;

public class IdentityEmailAssignedToAUserIntegrationEventConsumer : IConsumer<IdentityEmailAssignedToAUserIntegrationEvent>
{
    public async Task Consume(ConsumeContext<IdentityEmailAssignedToAUserIntegrationEvent> context)
    {

    }
}
