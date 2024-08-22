using Account.Contracts.IntegrationEvents;
using Identity.API.Domain.Entities;
using Identity.API.Infrastructure;
using Identity.Contracts.Entity;
using Identity.Contracts.Enumerations;
using Identity.Contracts.IntegrationEvent;
using MassTransit;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Identity.API.Consumers;

public class BusinessUserCreatedIntegrationEventConsumer(
    ApplicationDbContext dbContext, 
    UserManager<ApplicationUser> userManager,
    IPublishEndpoint publishEndpoint) : IConsumer<BusinessUserCreatedIntegrationEvent>
{
    private readonly ApplicationDbContext _dbContext = dbContext;
    private readonly UserManager<ApplicationUser> _userManager = userManager; 
    private readonly IPublishEndpoint _publishEndpoint = publishEndpoint;

    public async Task Consume(ConsumeContext<BusinessUserCreatedIntegrationEvent> context)
    {
        var strategy = _dbContext.Database.CreateExecutionStrategy();

        await strategy.Execute(async () =>
        {
            using var transaction = await _dbContext.Database.BeginTransactionAsync();  

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
                        IsAccountRegistered = true,
                        BusinessCode = context.Message.BusinessCode,
                        UserType = UserType.BusinessUser
                    });
                }

                foreach (var staffRequest in context.Message.Staffs ?? [])
                {
                    var duplicateStaff = duplicateStaffUsers.Where(x => x.PhoneNumber == staffRequest.PhoneNumber).FirstOrDefault();

                    if (duplicateStaff is not null)
                    {
                        duplicateUsers.Add(new(staffRequest.id, staffRequest.PhoneNumber, UserType.StaffUser));
                    } 
                    else
                    {
                        newUsers.Add(new ApplicationUser() 
                        {
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

                await _dbContext.Users.AddRangeAsync(newUsers);

                await transaction.CommitAsync();
            }
            catch (Exception)
            {
                throw;
            }
        });
    }
}
