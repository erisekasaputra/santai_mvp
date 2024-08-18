using Identity.API.Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace Identity.API.SeedWork;

public class SeedDatabase
{ 
    public static async Task Initialize(IServiceProvider serviceProvider)
    {
        var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
        var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();

        string[] roleNames = [.. UserTypes.AllTypes().Values];

        foreach (var roleName in roleNames)
        {
            var roleExist = await roleManager.RoleExistsAsync(roleName);
            if (!roleExist)
            {
                await roleManager.CreateAsync(new IdentityRole(roleName));
            }
        }
         
        var user = await userManager.FindByEmailAsync("erisekasaputra282000@gmail.com");
        if (user == null)
        {
            user = new ApplicationUser()
            {
                UserName = "erisekasaputra282000@gmail.com",
                Email = "erisekasaputra282000@gmail.com"
            };
            await userManager.CreateAsync(user, "000328Eris@");
        }
        await userManager.AddToRoleAsync(user, "Administrator");
    }
}
