using Identity.API.Domain.Entities;
using Identity.API.Enumerations;
using Identity.Contracts;
using Microsoft.AspNetCore.Identity;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Identity.API.SeedWork;

public class SeedDatabase
{ 
    public static async Task Initialize(IServiceProvider serviceProvider)
    {
        var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
        var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();

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


                var claims = new List<Claim>()
                {
                    new (JwtRegisteredClaimNames.Sub, user.Id),
                    new (JwtRegisteredClaimNames.Email, user.Email),
                    new (ClaimTypes.Name, user.UserName),
                    new (ClaimTypes.MobilePhone, user.PhoneNumber), 
                }; 

                foreach (var userType in GetUserTypeConfiguration.GetAll)
                {
                    await userManager.AddToRoleAsync(user, userType.ToString());
                    claims.Add(new Claim(ClaimTypes.Role, userType.ToString()));
                }

                await userManager.AddClaimsAsync(user, claims);
            } 
        }  
    }
}
