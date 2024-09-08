using System.Security.Claims;

namespace Identity.API.Service.Interfaces;

public interface IJwtTokenValidator
{
    Task<ClaimsPrincipal> ValidateAsync(string token);
}
