using Identity.API.Domain.Entities; 
using System.Security.Claims;

namespace Identity.API.Abstraction;

public interface ITokenService
{
    string GenerateAccessToken(ClaimsIdentity claims);
    Task<RefreshToken> GenerateRefreshTokenAsync(string userId);
    Task<RefreshToken?> GetStoredRefreshToken(string token);
    Task<RefreshToken?> RotateRefreshTokenAsync(string oldToken);
    Task<bool> InvalidateRefreshToken(string oldToken);
    bool ValidateTokenAsync(RefreshToken storedRefreshToken);
}
