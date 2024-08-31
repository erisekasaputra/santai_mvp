using Identity.API.Abstraction;
using Identity.API.Domain.Entities;
using Identity.API.Infrastructure;
using Identity.Contracts.Entity; 
using Identity.Contracts.IntegrationEvent;
using MassTransit;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore; 
using System.Data; 
namespace Identity.API.Consumers;

public class BusinessUserDeletedIntegrationEventConsumer(
    ApplicationDbContext dbContext,
    UserManager<ApplicationUser> userManager,
    IMediator mediator,
    ILogger<BusinessUserCreatedIntegrationEventConsumer> logger,
    ICacheService cacheService) : IConsumer<BusinessUserDeletedIntegrationEvent>
{
    private readonly ApplicationDbContext _dbContext = dbContext;
    private readonly UserManager<ApplicationUser> _userManager = userManager;
    private readonly IMediator _mediator = mediator;
    private readonly ILogger<BusinessUserCreatedIntegrationEventConsumer> _logger = logger;
    private readonly ICacheService _cacheService = cacheService;

    public async Task Consume(ConsumeContext<BusinessUserDeletedIntegrationEvent> context)
    {
        using var transaction = await _dbContext.Database.BeginTransactionAsync(IsolationLevel.ReadCommitted);

        var duplicateUsers = new List<DuplicateUser>();
        var newUsers = new List<ApplicationUser>();

        try
        {
            var businessUserId = context.Message.UserId.ToString();
            var staffIds = context.Message.Staffs?.Select(
                x => x.Id.ToString()) ?? [];

            var businessUser = await _dbContext.Users
                .Where(x => x.Id == businessUserId)
                    .FirstOrDefaultAsync(); 

            var staffUsers = await _dbContext.Users
              .Where(x => staffIds.Contains(x.Id))
                  .ToListAsync(); 

            var users = staffUsers.Append(businessUser);
 
            await DeleteUser([..users]);

            await _dbContext.SaveChangesAsync();

            await transaction.CommitAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.InnerException?.Message);
            await transaction.RollbackAsync();
        }
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
