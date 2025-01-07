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

public class UserAdmin
{
    public string Name { get; private set; }
    public string PhoneNumber { get; private  set; }
    public string Email { get; private set; }
    public string Password { get; private  set; }
    
    public UserAdmin(string name, string phoneNumber, string email, string password)
    {
        Name = name;
        PhoneNumber = phoneNumber;
        Email = email;
        Password = password;
    }
}

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
            var transaction = await dbContext.Database.BeginTransactionAsync();
            try
            { 
                UserType[] roleNames = [.. GetUserTypeConfiguration.GetAll];

                foreach (var roleName in roleNames)
                {
                    var roleExist = await roleManager.RoleExistsAsync(roleName.ToString());
                    if (!roleExist)
                    {
                        await roleManager.CreateAsync(new IdentityRole(roleName.ToString()));
                    }
                } 

                List<UserAdmin> newAdminUsers =
                [ 
                    new ("Irwan", "+601160706392", "irwan@santaitechnology.com", "Santai@admin24"),
                    new ("Fazz", "+60189888200", "fadzle@santaitechnology.com", "Santai@admin24"),
                ];


                foreach (var adminUser in newAdminUsers) 
                {   
                    var user = await userManager.FindByNameAsync(adminUser.PhoneNumber);

                    if (user is null)
                    {
                        user = await userManager.FindByEmailAsync(adminUser.Email);

                        if (user is null)
                        {
                            user = new ApplicationUser()
                            {
                                UserName = adminUser.PhoneNumber,
                                Email = adminUser.Email,
                                PhoneNumber = adminUser.PhoneNumber,
                                PhoneNumberConfirmed = true,
                                EmailConfirmed = true,
                                UserType = UserType.Administrator,
                                IsAccountRegistered = false,
                                DeviceIds = []
                            };

                            await userManager.CreateAsync(user, adminUser.Password);
                            await userManager.AddToRoleAsync(user, UserType.Administrator.ToString());

                            var userInfoLogin = new UserLoginInfo("google", googleOption.CurrentValue.ClientId, "google");
                            await userManager.AddLoginAsync(user, userInfoLogin);

                            var claims = new List<Claim>()
                            {
                                new (JwtRegisteredClaimNames.Sub, user.Id),
                                new (ClaimTypes.Email, adminUser.Email),
                                new (ClaimTypes.Name, adminUser.Name),
                                new (ClaimTypes.MobilePhone, adminUser.PhoneNumber),
                                new (ClaimTypes.Role, user.UserType.ToString()),
                                new (SantaiClaimTypes.UserType, user.UserType.ToString())
                            };

                            await userManager.AddClaimsAsync(user, claims);
                        }
                    }
                    else 
                    {   
                        var claims = new List<Claim>()
                        {
                            new (JwtRegisteredClaimNames.Sub, user.Id), 
                            new (ClaimTypes.Role, user.UserType.ToString()),
                            new (SantaiClaimTypes.UserType, user.UserType.ToString())
                        };
                        if (!string.IsNullOrEmpty(user.Email))
                        {
                            claims.Add(new Claim(ClaimTypes.Email, user.Email));
                        }
                        if (!string.IsNullOrEmpty(user.PhoneNumber))
                        {
                            claims.Add(new Claim(ClaimTypes.MobilePhone, user.PhoneNumber));
                        } 
                        if (!string.IsNullOrEmpty(user.UserName))
                        {
                            claims.Add(new Claim(ClaimTypes.Name, user.UserName));
                        }


                    






                        var newClaims = new List<Claim>()
                        {
                            new (JwtRegisteredClaimNames.Sub, user.Id),  
                            new (ClaimTypes.Role, user.UserType.ToString()),
                            new (SantaiClaimTypes.UserType, user.UserType.ToString())
                        };

                        if (!string.IsNullOrEmpty(adminUser.Email))
                        {
                            newClaims.Add(new Claim(ClaimTypes.Email, adminUser.Email));
                        }
                        if (!string.IsNullOrEmpty(adminUser.PhoneNumber))
                        {
                            newClaims.Add(new Claim(ClaimTypes.MobilePhone, adminUser.PhoneNumber));
                        } 
                        if (!string.IsNullOrEmpty(adminUser.Name))
                        {
                            newClaims.Add(new Claim(ClaimTypes.Name, adminUser.Name));
                        }






                        await userManager.RemoveClaimsAsync(user, claims); 
                        await userManager.AddClaimsAsync(user, newClaims);



                        var removePasswordResult = await userManager.RemovePasswordAsync(user);
                        if (removePasswordResult.Succeeded)
                        {
                            var addPasswordResult = await userManager.AddPasswordAsync(user, adminUser.Password); 
                        } 
                    }
                } 

                await dbContext.SaveChangesAsync(); 
                await transaction.CommitAsync();
            }
            catch (DbUpdateException)
            { 
                await transaction.RollbackAsync();
                throw;  
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                Console.WriteLine(ex.Message);
            }
            finally
            {
                if (transaction != null)
                {
                    await transaction.DisposeAsync();  
                }
            }
        }); 
    } 
}
