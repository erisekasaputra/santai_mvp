using Account.Contracts.IntegrationEvents;
using Identity.API.Abstraction;
using Identity.API.Domain.Entities;
using Identity.API.Infrastructure; 
using Identity.Contracts.Entity;
using Identity.Contracts.Enumerations;
using Identity.Contracts.IntegrationEvent;
using MassTransit;
using Microsoft.AspNetCore.Identity; 
using Microsoft.EntityFrameworkCore;
using SantaiClaimType;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Identity.API.Consumers;
 
public class BusinessUserCreatedIntegrationEventConsumer(
    ApplicationDbContext dbContext, 
    UserManager<ApplicationUser> userManager,
    IPublishEndpoint publishEndpoint,
    ILogger<BusinessUserCreatedIntegrationEventConsumer> logger,
    ICacheService cacheService) : IConsumer<BusinessUserCreatedIntegrationEvent>
{
    private readonly ApplicationDbContext _dbContext = dbContext;
    private readonly UserManager<ApplicationUser> _userManager = userManager; 
    private readonly IPublishEndpoint _publishEndpoint = publishEndpoint;
    private readonly ILogger<BusinessUserCreatedIntegrationEventConsumer> _logger = logger;
    private readonly ICacheService _cacheService = cacheService; 

    public async Task Consume(ConsumeContext<BusinessUserCreatedIntegrationEvent> context)
    {
        using var transaction = await _dbContext.Database.BeginTransactionAsync(IsolationLevel.Serializable);

        var duplicateUsers = new List<DuplicateUser>();
        var newUsers = new List<ApplicationUser>();

        try
        {
            var businessUserPhoneNumber = context.Message.PhoneNumber;
            var staffPhoneNumber = context.Message.Staffs?.Select(x => x.PhoneNumber) ?? [];

            var duplicateBusinessUser = await _dbContext.Users
                .Where(x => x.PhoneNumber == businessUserPhoneNumber)
                    .FirstOrDefaultAsync();


            var duplicateStaffUsers = await _dbContext.Users
              .Where(x => staffPhoneNumber.Contains(x.PhoneNumber))
                  .ToListAsync();

            if (duplicateBusinessUser is not null)
            {
                duplicateUsers.Add(new(context.Message.UserId, context.Message.PhoneNumber, UserType.BusinessUser));
            }
            else
            {
                newUsers.Add(new ApplicationUser()
                {
                    Id = context.Message.UserId.ToString(),
                    UserName = context.Message.PhoneNumber,
                    PhoneNumber = context.Message.PhoneNumber,
                    IsAccountRegistered = true,
                    BusinessCode = context.Message.BusinessCode,
                    UserType = UserType.BusinessUser
                });
            }

            foreach (var staffRequest in context.Message.Staffs ?? [])
            {
                var duplicateStaff = duplicateStaffUsers
                    .Where(x => x.PhoneNumber == staffRequest.PhoneNumber)
                    .FirstOrDefault();

                if (duplicateStaff is not null)
                {
                    duplicateUsers.Add(new(staffRequest.Id, staffRequest.PhoneNumber, UserType.StaffUser));
                }
                else
                {
                    newUsers.Add(new ApplicationUser()
                    {
                        Id = staffRequest.Id.ToString(),
                        UserName = staffRequest.PhoneNumber,
                        PhoneNumber = staffRequest.PhoneNumber,
                        IsAccountRegistered = true,
                        BusinessCode = context.Message.BusinessCode,
                        UserType = UserType.StaffUser
                    });
                }
            }


            if (duplicateUsers.Count > 0)
            {
                await _publishEndpoint.Publish(new PhoneNumberDuplicateIntegrationEvent(duplicateUsers));
            }

            foreach (var newUser in newUsers)
            {
                var result = await _userManager.CreateAsync(newUser, "000328Eris@");

                await _userManager.AddToRoleAsync(newUser, newUser.UserType.ToString());

                if (newUser.PhoneNumber is null)
                {
                    throw new ArgumentNullException(newUser.PhoneNumber);
                }

                var claims = new List<Claim>()
                {
                    new (JwtRegisteredClaimNames.Sub, newUser.Id),
                    new (ClaimTypes.Name, newUser.PhoneNumber),
                    new (ClaimTypes.MobilePhone, newUser.PhoneNumber),
                    new (SantaiClaimTypes.UserType, newUser.UserType.ToString()),
                    new (ClaimTypes.Role, newUser.UserType.ToString())
                };

                if (!string.IsNullOrWhiteSpace(newUser.BusinessCode))
                {
                    claims.Add(new Claim(SantaiClaimTypes.BusinessCode, newUser.BusinessCode));
                }

                await _userManager.AddClaimsAsync(newUser, claims);
            }

            await _dbContext.SaveChangesAsync();

            await transaction.CommitAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.InnerException?.Message);
            await transaction.RollbackAsync();
        }
    } 
}
