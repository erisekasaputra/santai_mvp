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
    Task<bool> BlackListRefreshTokenAsync(string refreshToken);
    Task<bool> BlackListAccessTokenAsync(string accessToken);
    Task<bool> IsRefreshTokenBlacklisted(string refreshToken);    
    Task<bool> IsAccessTokenBlacklisted(string accessToken);    
}
