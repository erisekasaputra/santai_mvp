using Core.Models; 

namespace Identity.API.Service.Interfaces;

public interface ITokenCacheService
{
    Task<RefreshToken?> GetStoredRefreshToken(string token); 
    Task<RefreshToken> SaveRefreshToken(RefreshToken refreshToken);  
    Task<RefreshToken?> RotateRefreshTokenAsync(string oldToken);  
    bool ValidateToken(RefreshToken storedRefreshToken); 
    Task<bool> BlackListRefreshTokenAsync(string refreshToken); 
    Task<bool> BlackListAccessTokenAsync(string accessToken); 
    Task<bool> IsRefreshTokenBlacklisted(string refreshToken); 
    Task<bool> IsAccessTokenBlacklisted(string accessToken);
}
