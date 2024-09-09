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

public class BusinessUserCreatedIntegrationEventConsumer(
    ApplicationDbContext dbContext, 
    UserManager<ApplicationUser> userManager,
    IMediator mediator,
    ILogger<BusinessUserCreatedIntegrationEventConsumer> logger) : IConsumer<BusinessUserCreatedIntegrationEvent>
{
    private readonly ApplicationDbContext _dbContext = dbContext;
    private readonly UserManager<ApplicationUser> _userManager = userManager; 
    private readonly IMediator _mediator = mediator;
    private readonly ILogger<BusinessUserCreatedIntegrationEventConsumer> _logger = logger;
      
    public async Task Consume(ConsumeContext<BusinessUserCreatedIntegrationEvent> context)
    {
        var strategy = _dbContext.Database.CreateExecutionStrategy();

        await strategy.ExecuteAsync(async () =>
        {
            using var transaction = await _dbContext.Database.BeginTransactionAsync(IsolationLevel.ReadCommitted);

            var duplicateUsers = new List<ApplicationUser>();
            var users = new List<(ApplicationUser users, string password)>();

            try
            {
                var businessUser = context.Message;
                var staffUsers = businessUser.Staffs;

                var businessUserPhoneNumber = businessUser.PhoneNumber;
                var staffPhoneNumber = staffUsers?.Select(x => x.PhoneNumber) ?? [];

                var duplicateBusinessUser = await _dbContext.Users
                    .Where(x => x.PhoneNumber == businessUserPhoneNumber)
                        .FirstOrDefaultAsync();


                var duplicateStaffUsers = await _dbContext.Users
                  .Where(x => staffPhoneNumber.Contains(x.PhoneNumber)).ToListAsync();


                if (duplicateBusinessUser is not null && businessUser.UserId != Guid.Parse(duplicateBusinessUser.Id))
                {
                    duplicateUsers.Add(duplicateBusinessUser);
                }

                if (duplicateBusinessUser is not null && !duplicateBusinessUser.IsAccountRegistered)
                {
                    duplicateBusinessUser.IsAccountRegistered = true;
                    await _userManager.UpdateAsync(duplicateBusinessUser);
                }

                if (duplicateBusinessUser is null)
                {
                    users.Add((new ApplicationUser()
                    {
                        Id = businessUser.UserId.ToString(),
                        UserName = businessUser.PhoneNumber,
                        PhoneNumber = businessUser.PhoneNumber,
                        IsAccountRegistered = true,
                        BusinessCode = businessUser.BusinessCode,
                        UserType = UserType.BusinessUser
                    },
                    businessUser.Password));
                }

                var listUpdatedStaff = new List<ApplicationUser>();

                foreach (var staff in context.Message.Staffs ?? [])
                {
                    var duplicateStaff = duplicateStaffUsers
                        .Where(x => x.PhoneNumber == staff.PhoneNumber)
                        .FirstOrDefault();

                    if (duplicateStaff is not null && staff.Id != Guid.Parse(duplicateStaff.Id))
                    {
                        duplicateUsers.Add(duplicateStaff);
                    }

                    if (duplicateStaff is not null && !duplicateStaff.IsAccountRegistered)
                    {
                        duplicateStaff.IsAccountRegistered = true;
                        listUpdatedStaff.Add(duplicateStaff);
                    }

                    if (duplicateStaff is null)
                    {
                        users.Add((new()
                        {
                            Id = staff.Id.ToString(),
                            UserName = staff.PhoneNumber,
                            PhoneNumber = staff.PhoneNumber,
                            IsAccountRegistered = true,
                            BusinessCode = staff.BusinessCode,
                            UserType = UserType.StaffUser
                        },
                        staff.Password));
                    }
                } 

                if (listUpdatedStaff.Any())
                {
                    _dbContext.UpdateRange(listUpdatedStaff);
                }

                if (duplicateUsers.Count > 0)
                {
                    await _mediator.Publish(
                        new PhoneNumberDuplicateDomainEvent(duplicateUsers));
                }

                if (users.Count == 0)
                {
                    await _dbContext.SaveChangesAsync();

                    await transaction.CommitAsync();

                    return;
                }

                foreach ((var user, var password) in users)
                {
                    var result = await _userManager.CreateAsync(user, password);

                    if (!result.Succeeded)
                    {
                        foreach (var error in result.Errors)
                        {
                            _logger.LogError("An error occured during save new staff user {id}: {errors}", user.Id, error);
                        }

                        continue;
                    }

                    var resultRole = await _userManager.AddToRoleAsync(user, user.UserType.ToString());

                    if (!resultRole.Succeeded)
                    {
                        foreach (var error in resultRole.Errors)
                        {
                            _logger.LogError("An error occured during save new staff user role {id}: {errors}", user.Id, error);
                        }

                        continue;
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
                        claims.Add(new(SantaiClaimTypes.BusinessCode, user.BusinessCode));
                    }

                    var resultClaim = await _userManager.AddClaimsAsync(user, claims);

                    if (!resultClaim.Succeeded)
                    {
                        foreach (var error in resultClaim.Errors)
                        {
                            _logger.LogError("An error occured during save new staff user claim {id}: {errors}", user.Id, error.Description);
                        }

                        continue;
                    }
                }

                await _dbContext.SaveChangesAsync();

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
