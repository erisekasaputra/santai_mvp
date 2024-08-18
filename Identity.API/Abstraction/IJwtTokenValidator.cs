using System.Security.Claims;

namespace Identity.API.Abstraction;

public interface IJwtTokenValidator
{
    Task<ClaimsPrincipal> ValidateAsync(string token);
}
