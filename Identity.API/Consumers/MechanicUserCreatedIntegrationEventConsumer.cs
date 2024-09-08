using Identity.API.Domain.Entities;
using Identity.API.Infrastructure; 
using Identity.Contracts.IntegrationEvent;
using MassTransit;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore; 

namespace Identity.API.Consumers;

public class MechanicUserCreatedIntegrationEventConsumer(
    ApplicationDbContext dbContext,
    UserManager<ApplicationUser> userManager,
    IMediator mediator,
    ILogger<BusinessUserCreatedIntegrationEventConsumer> logger) :  IConsumer<MechanicUserCreatedIntegrationEvent>
{
    private readonly ApplicationDbContext _dbContext = dbContext;
    private readonly UserManager<ApplicationUser> _userManager = userManager;
    private readonly IMediator _mediator = mediator;
    private readonly ILogger<BusinessUserCreatedIntegrationEventConsumer> _logger = logger; 

    public async Task Consume(ConsumeContext<MechanicUserCreatedIntegrationEvent> context)
    {
        var strategy = _dbContext.Database.CreateExecutionStrategy();

        await strategy.ExecuteAsync(async () =>
        {
            using var transaction = await _dbContext.Database.BeginTransactionAsync();

            try
            {
                var userId = context.Message.UserId;
                var userPhoneNumber = context.Message.PhoneNumber;

                var user = await _userManager.FindByIdAsync(userId.ToString());

                if (user is null || userPhoneNumber != user.PhoneNumber)
                {
                    await _mediator.Publish(new MechanicUserDeletedIntegrationEvent(userId));
                    return;
                }

                user.IsAccountRegistered = true;

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
