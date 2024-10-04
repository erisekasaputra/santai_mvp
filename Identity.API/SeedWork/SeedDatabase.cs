using Core.Configurations;
using Core.CustomClaims;
using Core.Enumerations; 
using Identity.API.Domain.Entities;
using Identity.API.Infrastructure;  
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Identity.API.SeedWork;

public class SeedDatabase
{ 
    public static async Task Initialize(IServiceProvider serviceProvider)
    {
        var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
        var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
        var googleOption = serviceProvider.GetRequiredService<IOptionsMonitor<GoogleSSOConfiguration>>();
        var dbContext = serviceProvider.GetRequiredService<ApplicationDbContext>(); 

        var strategy = dbContext.Database.CreateExecutionStrategy();

        await strategy.ExecuteAsync(async () =>
        {
            try
            {
                var transaction = await dbContext.Database.BeginTransactionAsync();

                UserType[] roleNames = [.. GetUserTypeConfiguration.GetAll];

                foreach (var roleName in roleNames)
                {
                    var roleExist = await roleManager.RoleExistsAsync(roleName.ToString());
                    if (!roleExist)
                    {
                        await roleManager.CreateAsync(new IdentityRole(roleName.ToString()));
                    }
                }

                const string phoneNumber = "+6285791387558";
                const string email = "erisekasaputra282000@gmail.com";
                const string password = "000328Eris@";

                var user = await userManager.FindByNameAsync(phoneNumber);

                if (user is null)
                {
                    user = await userManager.FindByEmailAsync(email);

                    if (user is null)
                    {
                        user = new ApplicationUser()
                        {
                            UserName = phoneNumber,
                            Email = email,
                            PhoneNumber = phoneNumber,
                            PhoneNumberConfirmed = true,
                            EmailConfirmed = true,
                            UserType = UserType.Administrator,
                            IsAccountRegistered = false,
                            DeviceIds = []
                        };

                        await userManager.CreateAsync(user, password);
                        await userManager.AddToRoleAsync(user, UserType.Administrator.ToString());

                        var userInfoLogin = new UserLoginInfo("google", googleOption.CurrentValue.ClientId, "google");

                        await userManager.AddLoginAsync(user, userInfoLogin);

                        var claims = new List<Claim>()
                        {
                            new (JwtRegisteredClaimNames.Sub, user.Id),
                            new (ClaimTypes.Email, user.Email),
                            new (ClaimTypes.Name, user.UserName),
                            new (ClaimTypes.MobilePhone, user.PhoneNumber),
                            new (ClaimTypes.Role, user.UserType.ToString()),
                            new (SantaiClaimTypes.UserType, user.UserType.ToString())
                        };

                        await userManager.AddClaimsAsync(user, claims);
                    }
                }

                await dbContext.SaveChangesAsync();

                await transaction.CommitAsync();
            }
            catch (DbUpdateException)
            {
                throw;  
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }); 
    } 
}
