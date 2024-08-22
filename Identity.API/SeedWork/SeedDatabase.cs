using Identity.API.Configs;
using Identity.API.Domain.Entities;
using Identity.API.Enumerations;
using Identity.Contracts.Enumerations;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using SantaiClaimType;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Identity.API.SeedWork;

public class SeedDatabase
{ 
    public static async Task Initialize(IServiceProvider serviceProvider)
    {
        var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
        var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
        var googleOption = serviceProvider.GetRequiredService<IOptionsMonitor<GoogleConfig>>();

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
                    UserType = UserType.Administrator 
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
    } 
}
