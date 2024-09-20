using Core.Configurations;
using Core.CustomClaims;
using Core.Enumerations;
using Core.Models;
using Core.SeedWorks;
using Core.Services.Interfaces;
using Core.Utilities;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Core.Services;

public class JwtTokenService : ITokenService
{
    private readonly IUserInfoService _userInfoService;
    private readonly IOptionsMonitor<JwtConfiguration> _jwtConfigs;
    private readonly ICacheService _cacheService;
    public JwtTokenService(
        IUserInfoService userInfoService,
        IOptionsMonitor<JwtConfiguration> jwtConfigs, 
        ICacheService cacheService)
    {
        _userInfoService = userInfoService;
        _jwtConfigs = jwtConfigs;
        _cacheService = cacheService;
    }

    public string GenerateAccessToken(ClaimsIdentity claims)
    {
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtConfigs.CurrentValue.SecretKey));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = claims,
            Expires = DateTime.UtcNow.AddSeconds(_jwtConfigs.CurrentValue.TotalSecondsAccessTokenLifetime),
            Issuer = _jwtConfigs.CurrentValue.Issuer,
            Audience = _jwtConfigs.CurrentValue.Audience,
            SigningCredentials = credentials,
            IssuedAt = DateTime.UtcNow,
            NotBefore = DateTime.UtcNow
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }

    public string GenerateAccessTokenForServiceToService()
    { 
        var userClaim = _userInfoService.GetUserInfo(); 
        if (userClaim is null)
        {
            return string.Empty;
        }

        var claims = new List<Claim>()
        {
            new (JwtRegisteredClaimNames.Sub, userClaim.Sub.ToString()),
            new (JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new (SantaiClaimTypes.UserType, userClaim.CurrentUserType.ToString()),
            new (ClaimTypes.Role, UserType.ServiceToService.ToString())
        };

        var claimIdentity = new ClaimsIdentity(claims);  
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtConfigs.CurrentValue.SecretKey));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = claimIdentity,
            Expires = DateTime.UtcNow.AddSeconds(_jwtConfigs.CurrentValue.TotalSecondsAccessTokenLifetime),
            Issuer = _jwtConfigs.CurrentValue.Issuer,
            Audience = _jwtConfigs.CurrentValue.Audience,
            SigningCredentials = credentials,
            IssuedAt = DateTime.UtcNow,
            NotBefore = DateTime.UtcNow
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var newToken = tokenHandler.CreateToken(tokenDescriptor);

        var generatedToken = tokenHandler.WriteToken(newToken);  

        return generatedToken;
    }

    public async Task<RefreshToken> GenerateRefreshTokenAsync(string userId)
    {
        var newToken = SecretGenerator.GenerateRandomSecret();
        var tokenExpiry = DateTime.UtcNow.AddDays(_jwtConfigs.CurrentValue.TotalDaysRefreshTokenLifetime); 

        var hashedRefreshToken = new RefreshToken()
        {
            Token = newToken.HashToken(),
            UserId = userId,
            ExpiryDateUtc = tokenExpiry
        };

        var key = CacheKey.RefreshTokenCacheKey(newToken.HashToken());
        await _cacheService.SetAsync(key, hashedRefreshToken, TimeSpan.FromDays(_jwtConfigs.CurrentValue.TotalDaysRefreshTokenLifetime)); 

        return new RefreshToken()
        {
            Token = newToken,
            UserId = userId,
            ExpiryDateUtc = tokenExpiry
        };
    }


    public async Task<RefreshToken?> GetStoredRefreshToken(string token)
    {
        var key = CacheKey.RefreshTokenCacheKey(token.HashToken());
        var storedToken = await _cacheService.GetAsync<RefreshToken>(key);

        return storedToken;
    }


    public async Task<RefreshToken?> RotateRefreshTokenAsync(string oldToken)
    {
        var storedRefreshToken = await GetStoredRefreshToken(oldToken) ?? throw new SecurityTokenException("Invalid or expired refresh token");

        if (!ValidateTokenAsync(storedRefreshToken))
        {
            throw new SecurityTokenException("Invalid or expired refresh token");
        }

        await InvalidateRefreshToken(oldToken);

        var newRefreshToken = await GenerateRefreshTokenAsync(storedRefreshToken.UserId);

        return newRefreshToken;
    }

    public async Task<bool> InvalidateRefreshToken(string oldToken)
    {
        var cacheKey = CacheKey.RefreshTokenCacheKey(oldToken.HashToken());

        return await _cacheService.DeleteAsync(cacheKey);
    }

    public bool ValidateTokenAsync(RefreshToken storedRefreshToken)
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
            CacheKey.BlackListRefreshTokenKey(refreshToken),
                new { Blacklisted = true },
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
