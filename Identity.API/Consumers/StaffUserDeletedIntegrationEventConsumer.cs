using Identity.API.Domain.Entities;
using Identity.API.Infrastructure;
using Identity.Contracts.IntegrationEvent;
using MassTransit; 
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace Identity.API.Consumers;

public class StaffUserDeletedIntegrationEventConsumer(
    ApplicationDbContext dbContext,
    UserManager<ApplicationUser> userManager, 
    ILogger<BusinessUserCreatedIntegrationEventConsumer> logger) : IConsumer<StaffUserDeletedIntegrationEvent>
{
    private readonly ApplicationDbContext _dbContext = dbContext;
    private readonly UserManager<ApplicationUser> _userManager = userManager; 
    private readonly ILogger<BusinessUserCreatedIntegrationEventConsumer> _logger = logger;

    public async Task Consume(ConsumeContext<StaffUserDeletedIntegrationEvent> context)
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

                await DeleteUser([user]);

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

    public async Task DeleteUser(params ApplicationUser?[] users)
    {
        foreach (var user in users)
        {
            if (user is null)
            {
                continue;
            }

            // Fetch roles, claims, and logins concurrently
            var rolesTask = _userManager.GetRolesAsync(user);
            var claimsTask = _userManager.GetClaimsAsync(user);
            var loginsTask = _userManager.GetLoginsAsync(user);

            // Wait for all tasks to complete
            await Task.WhenAll(rolesTask, claimsTask, loginsTask);

            var roles = rolesTask.Result;
            var claims = claimsTask.Result;
            var logins = loginsTask.Result;

            // Remove roles and claims concurrently
            var removeRolesTask = _userManager.RemoveFromRolesAsync(user, roles);
            var removeClaimsTask = _userManager.RemoveClaimsAsync(user, claims);

            await Task.WhenAll(removeRolesTask, removeClaimsTask);

            foreach (var login in logins)
            {
                await _userManager.RemoveLoginAsync(user, login.LoginProvider, login.ProviderKey);
            }

            await _userManager.DeleteAsync(user);
        }
    }
}
