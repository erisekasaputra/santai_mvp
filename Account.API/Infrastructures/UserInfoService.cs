using Account.API.Model;
using Identity.Contracts;
using SantaiClaimType;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Account.API.Infrastructures;

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
             
            var sub = httpContext.User.FindFirstValue(JwtRegisteredClaimNames.Sub); 
            var phoneNumber = httpContext.User.FindFirstValue(ClaimTypes.MobilePhone); 
            var email = httpContext.User.FindFirstValue(ClaimTypes.Email); 
            var businessCode = httpContext.User.FindFirstValue(SantaiClaimTypes.BusinessCode);

            var rawUserType = httpContext.User.FindFirstValue(SantaiClaimTypes.UserType);

            if (string.IsNullOrWhiteSpace(rawUserType))
            {
                return null;
            }

            var userType = Enum.Parse<UserType>(rawUserType);

            if (sub == default || string.IsNullOrWhiteSpace(phoneNumber) || string.IsNullOrWhiteSpace(businessCode))
            {
                return null;
            }

            var userClaim = new UserClaim(Guid.Parse(sub), phoneNumber, email, userType, businessCode);

            return userClaim;
        }

        return null;
    }
}