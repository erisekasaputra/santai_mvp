using Identity.API.Domain.Entities;
using Identity.API.Infrastructure;
using Identity.Contracts.IntegrationEvent;
using MassTransit;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace Identity.API.Consumers;

public class MechanicUserDeletedIntegrationEventConsumer(
    ApplicationDbContext dbContext,
    UserManager<ApplicationUser> userManager,
    ILogger<BusinessUserCreatedIntegrationEventConsumer> logger) : IConsumer<MechanicUserDeletedIntegrationEvent>
{
    private readonly ApplicationDbContext _dbContext = dbContext;
    private readonly UserManager<ApplicationUser> _userManager = userManager; 
    private readonly ILogger<BusinessUserCreatedIntegrationEventConsumer> _logger = logger; 
    public async Task Consume(ConsumeContext<MechanicUserDeletedIntegrationEvent> context)
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

            await _userManager.DeleteAsync(user);

            await transaction.CommitAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.InnerException?.Message);
            await transaction.RollbackAsync();
        }
    }
}
