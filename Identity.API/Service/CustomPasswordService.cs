using Identity.API.Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace Identity.API.Service;

public class CustomPasswordService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IPasswordValidator<ApplicationUser> _passwordValidator;
    private readonly IPasswordHasher<ApplicationUser> _passwordHasher;

    public CustomPasswordService(
        UserManager<ApplicationUser> userManager,
        IPasswordValidator<ApplicationUser> passwordValidator,
        IPasswordHasher<ApplicationUser> passwordHasher)
    {
        _userManager = userManager;
        _passwordValidator = passwordValidator;
        _passwordHasher = passwordHasher;
    }
}
