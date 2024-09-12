using Core.Enumerations;
using Core.Models;
using System.Security.Claims;

namespace Core.Services.Interfaces;

public interface ITokenService
{
    string GenerateAccessToken(ClaimsIdentity claims);
    Task<string> GenerateAccessTokenForServiceToService(
        UserType userType,
        string serviceKey,
        bool forceNewToken = false);
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
