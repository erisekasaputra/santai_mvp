using Account.Infrastructure;
using Identity.Contracts.IntegrationEvent;
using MassTransit;

namespace Account.API.Consumers;

public class IdentityPhoneNumberConfirmedIntegrationEventConsumer : IConsumer<IdentityPhoneNumberConfirmedIntegrationEvent>
{
    private readonly AccountDbContext _dbContext;
    public IdentityPhoneNumberConfirmedIntegrationEventConsumer(AccountDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task Consume(ConsumeContext<IdentityPhoneNumberConfirmedIntegrationEvent> context)
    {
        await Task.Delay(1);
    }
}
