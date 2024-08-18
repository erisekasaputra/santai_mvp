using Identity.API.Abstraction;
using Identity.API.Configs;
using Identity.API.Domain.Entities;
using Identity.API.SeedWork;
using Identity.API.Utilities;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims; 
using System.Text;

namespace Identity.API.Service;

public class JwtTokenService : ITokenService
{
    private readonly IOptionsMonitor<JwtConfig> _jwtConfigs;
    private readonly ICacheService _cacheService;
    public JwtTokenService(IOptionsMonitor<JwtConfig> jwtConfigs, ICacheService cacheService)
    {
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
            Expires = DateTime.UtcNow.AddHours(1),
            Issuer = _jwtConfigs.CurrentValue.Issuer,
            Audience = _jwtConfigs.CurrentValue.Audience,
            SigningCredentials = credentials,
            IssuedAt = DateTime.UtcNow
        }; 

        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }

    public async Task<RefreshToken> GenerateRefreshTokenAsync(string userId)
    {
        var newToken = SecretGenerator.GenerateRandomSecret();  
        var tokenExpiry = DateTime.UtcNow.AddMonths(1);


        var hashedRefreshToken = new RefreshToken()
        {
            Token = newToken.HashToken(),
            UserId = userId,
            ExpiryDateUtc = tokenExpiry
        };

        var key = CacheKey.RefreshTokenCacheKey(newToken.HashToken());
        await _cacheService.SetAsync(key, hashedRefreshToken, TimeSpan.FromDays(30));



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
        var storedRefreshToken = await GetStoredRefreshToken(oldToken);

        if (storedRefreshToken is null)
        {
            throw new SecurityTokenException("Invalid or expired refresh token");
        }

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

        return (storedRefreshToken.ExpiryDateUtc > DateTime.UtcNow);
    }

}
