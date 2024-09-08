using Catalog.API.Models;
using Catalog.API.Services.Interfaces;
using Identity.Contracts.Enumerations; 
using SantaiClaimType;
using System.Security.Claims;

namespace Catalog.API.Services;

public class UserService : IUserInfoService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public UserService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public UserClaim? GetUserInfoAsync()
    {
        var httpContext = _httpContextAccessor.HttpContext;

        if (httpContext != null && httpContext.User.Identity?.IsAuthenticated == true)
        {
            var sub = httpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var phoneNumber = httpContext.User.FindFirstValue(ClaimTypes.MobilePhone);
            var email = httpContext.User.FindFirstValue(ClaimTypes.Email);
            var businessCode = httpContext.User.FindFirstValue(SantaiClaimTypes.BusinessCode);
            var rawUserType = httpContext.User.FindFirstValue(SantaiClaimTypes.UserType);

            if (string.IsNullOrWhiteSpace(rawUserType))
            {
                return null;
            }

            var userType = Enum.Parse<UserType>(rawUserType);

            if (sub == default || string.IsNullOrWhiteSpace(phoneNumber))
            {
                return null;
            }

            if (userType == UserType.StaffUser && string.IsNullOrWhiteSpace(businessCode))
            {
                return null;
            }

            var userClaim = new UserClaim(Guid.Parse(sub), phoneNumber, email, userType, businessCode);

            return userClaim;
        }

        return null;
    }
}