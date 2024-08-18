using System.Security.Claims;

namespace Identity.API.Abstraction;

public interface IGoogleTokenValidator
{
    Task<ClaimsPrincipal> ValidateAsync(string token);
}
