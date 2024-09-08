using Core.Events;
using Identity.API.Domain.Entities;
using Identity.API.Infrastructure; 
using MassTransit;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace Identity.API.Consumers;

public class RegularUserDeletedIntegrationEventConsumer(
    ApplicationDbContext dbContext,
    UserManager<ApplicationUser> userManager,
    ILogger<BusinessUserCreatedIntegrationEventConsumer> logger) : IConsumer<RegularUserDeletedIntegrationEvent>
{
    private readonly ApplicationDbContext _dbContext = dbContext;
    private readonly UserManager<ApplicationUser> _userManager = userManager;
    private readonly ILogger<BusinessUserCreatedIntegrationEventConsumer> _logger = logger;

    public async Task Consume(ConsumeContext<RegularUserDeletedIntegrationEvent> context)
    {
        var strategy = _dbContext.Database.CreateExecutionStrategy();

        await strategy.ExecuteAsync(async () =>
        {
            using var transaction = await _dbContext.Database.BeginTransactionAsync(IsolationLevel.ReadCommitted);
            try
            {
                var userId = context.Message.UserId;

                var user = await _userManager.FindByIdAsync(userId.ToString());

                if (user is null)
                {
                    return;
                }

                user.IsAccountRegistered = false;

                await _userManager.UpdateAsync(user);

                await transaction.CommitAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.InnerException?.Message);
                await transaction.RollbackAsync();
                throw;
            }
        }); 
    }
}
