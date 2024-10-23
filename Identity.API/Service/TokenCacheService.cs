using Core.Configurations;
using Core.Models;
using Core.Services.Interfaces;
using Core.Utilities;
using Identity.API.SeedWork;
using Identity.API.Service.Interfaces;
using Microsoft.Extensions.Options; 

namespace Identity.API.Service;

public class TokenCacheService(
    IOptionsMonitor<JwtConfiguration> jwtConfigs,
    ICacheService cacheService) : ITokenCacheService
{ 
    private readonly IOptionsMonitor<JwtConfiguration> _jwtConfigs = jwtConfigs;
    private readonly ICacheService _cacheService = cacheService;
     
    public async Task<RefreshToken> SaveRefreshToken(RefreshToken refreshToken)
    {
        var key = CacheKey.RefreshTokenCacheKey(refreshToken.Token);
        await _cacheService.SetAsync(key, refreshToken, TimeSpan.FromDays(_jwtConfigs.CurrentValue.TotalDaysRefreshTokenLifetime));
        return refreshToken;
    }

    public async Task<RefreshToken?> GetStoredRefreshToken(string refreshToken)
    {
        var key = CacheKey.RefreshTokenCacheKey(refreshToken);
        var storedToken = await _cacheService.GetAsync<RefreshToken>(key);
        return storedToken;
    }


    public async Task<RefreshToken?> RotateRefreshTokenAsync(string oldToken)
    {  
        var storedRefreshToken = await GetStoredRefreshToken(oldToken);

        if (storedRefreshToken is null)
        {
            return null;
        }

        if (!ValidateToken(storedRefreshToken))
        {
            return null;
        }

        //await InvalidateRefreshToken(oldToken);
        return storedRefreshToken;
    }

    public async Task<bool> InvalidateRefreshToken(string oldToken)
    {
        var cacheKey = CacheKey.RefreshTokenCacheKey(oldToken); 
        return await _cacheService.DeleteAsync(cacheKey);
    }

    public bool ValidateToken(RefreshToken storedRefreshToken)
    {
        if (storedRefreshToken is null)
        {
            return false;
        }

        return storedRefreshToken.ExpiryDateUtc > DateTime.UtcNow;
    }

    public async Task<bool> BlackListRefreshTokenAsync(string refreshToken)
    {
        return await _cacheService.SetAsync(
            CacheKey.BlackListRefreshTokenKey(refreshToken), refreshToken,
                TimeSpan.FromDays(_jwtConfigs.CurrentValue.TotalDaysRefreshTokenLifetime));
    }

    public async Task<bool> BlackListAccessTokenAsync(string accessToken)
    {
        return await _cacheService.SetAsync(
            CacheKey.BlackListAccessTokenKey(accessToken), accessToken,
                TimeSpan.FromSeconds(_jwtConfigs.CurrentValue.TotalSecondsAccessTokenLifetime));
    }

    public async Task<bool> IsRefreshTokenBlacklisted(string refreshToken)
    {
        return await _cacheService.GetAsync<string>(CacheKey.BlackListRefreshTokenKey(refreshToken)) is not null;
    }

    public async Task<bool> IsAccessTokenBlacklisted(string accessToken)
    {
        return await _cacheService.GetAsync<string>(CacheKey.BlackListAccessTokenKey(accessToken)) is not null;
    }
}
