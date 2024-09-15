using Core.CustomClaims;
using Core.Enumerations;
using Core.Events;
using Core.Models;
using Identity.API.Domain.Entities;
using Identity.API.Domain.Events;
using Identity.API.Infrastructure; 
using MassTransit;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
namespace Identity.API.Consumers;

public class StaffUserCreatedIntegrationEventConsumer(
    ApplicationDbContext dbContext,
    UserManager<ApplicationUser> userManager,
    IMediator mediator,
    ILogger<StaffUserCreatedIntegrationEventConsumer> logger) : IConsumer<StaffUserCreatedIntegrationEvent>
{
    private readonly ApplicationDbContext _dbContext = dbContext;
    private readonly UserManager<ApplicationUser> _userManager = userManager;
    private readonly IMediator _mediator = mediator;
    private readonly ILogger<StaffUserCreatedIntegrationEventConsumer> _logger = logger; 
    public async Task Consume(ConsumeContext<StaffUserCreatedIntegrationEvent> context)
    {
        var strategy = _dbContext.Database.CreateExecutionStrategy();

        await strategy.ExecuteAsync(async () =>
        {
            using var transaction = await _dbContext.Database.BeginTransactionAsync(IsolationLevel.ReadCommitted);
             
            try
            {
                var staff = context.Message.Staff;

                var duplicateStaff = await _dbContext.Users
                  .Where(x => staff.PhoneNumber == x.PhoneNumber)
                      .FirstOrDefaultAsync(); 

                if (duplicateStaff is not null)
                {
                    if (staff.Id != Guid.Parse(duplicateStaff.Id))
                    {
                        DuplicateUser duplicateUser = new (staff.Id, staff.PhoneNumber, UserType.StaffUser); 
                        await _mediator.Publish(
                            new PhoneNumberDuplicateDomainEvent(
                                [duplicateUser])); 
                        await _dbContext.SaveChangesAsync(); 
                        await transaction.CommitAsync();
                    } 
                    else if (!duplicateStaff.IsAccountRegistered)
                    {
                        duplicateStaff.IsAccountRegistered = true; 
                        await _userManager.UpdateAsync(duplicateStaff); 
                        await _dbContext.SaveChangesAsync(); 
                        await transaction.CommitAsync();
                    }
                } 
                else
                {
                    var user = new ApplicationUser()
                    {
                        Id = staff.Id.ToString(),
                        UserName = staff.PhoneNumber,
                        PhoneNumber = staff.PhoneNumber,
                        IsAccountRegistered = true,
                        BusinessCode = staff.BusinessCode,
                        UserType = UserType.BusinessUser
                    };

                    var result = await _userManager.CreateAsync(user, staff.Password);

                    if (!result.Succeeded)
                    {
                        _logger.LogError("An error occured during save new staff user {id}: {errors}", user.Id, result.Errors);
                        return;
                    }

                    var resultRole = await _userManager.AddToRoleAsync(user, user.UserType.ToString());

                    if (!resultRole.Succeeded)
                    {
                        _logger.LogError("An error occured during save new staff user role {id}: {errors}", user.Id, resultRole.Errors);
                        return;
                    }

                    if (string.IsNullOrWhiteSpace(user.PhoneNumber))
                    {
                        throw new ArgumentNullException(user.PhoneNumber);
                    }

                    var claims = new List<Claim>()
                    {
                        new (JwtRegisteredClaimNames.Sub, user.Id),
                        new (ClaimTypes.Name, user.PhoneNumber),
                        new (ClaimTypes.MobilePhone, user.PhoneNumber),
                        new (SantaiClaimTypes.UserType, user.UserType.ToString()),
                        new (ClaimTypes.Role, user.UserType.ToString())
                    };

                    if (!string.IsNullOrWhiteSpace(user.BusinessCode))
                    {
                        claims.Add(new Claim(SantaiClaimTypes.BusinessCode, user.BusinessCode));
                    }

                    var resultClaim = await _userManager.AddClaimsAsync(user, claims);

                    if (!resultClaim.Succeeded)
                    {
                        _logger.LogError("An error occured during save new staff user claim {id}: {errors}", user.Id, resultClaim.Errors);
                        return;
                    }

                    await _dbContext.SaveChangesAsync();

                    await transaction.CommitAsync();
                }
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
