using Core.CustomClaims;
using Core.Enumerations;
using Core.Models;
using Core.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace Core.Services;

public class UserInfoService : IUserInfoService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public UserInfoService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public ServiceClaim? GetServiceInfoAsync()
    {
        var httpContext = _httpContextAccessor.HttpContext;

        if (httpContext != null && httpContext.User.Identity?.IsAuthenticated == true)
        {  
            var rawUserType = httpContext.User.FindFirstValue(SantaiClaimTypes.UserType); 
            var rawUserRole = httpContext.User.FindFirstValue(ClaimTypes.Role); 

            if (string.IsNullOrWhiteSpace(rawUserType) 
                || string.IsNullOrWhiteSpace(rawUserRole))
            {
                return null;
            }
             
            var userType = Enum.Parse<UserType>(rawUserType); 
            var userRole = Enum.Parse<UserType>(rawUserRole); 

            if (userRole != UserType.ServiceToService)
            {
                return null;
            }

            var userClaim = new ServiceClaim(userType); 
            return userClaim;
        }

        return null;
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
            var role = httpContext.User.FindFirstValue(ClaimTypes.Role);

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

            var userClaim = new UserClaim(Guid.Parse(sub), phoneNumber, email, userType, businessCode, role);

            return userClaim;
        }

        return null;
    }
}
