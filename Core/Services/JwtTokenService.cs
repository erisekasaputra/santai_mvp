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

public class JwtTokenService(
    IUserInfoService userInfoService,
    IOptionsMonitor<JwtConfiguration> jwtConfigs,
    ICacheService cacheService) : ITokenService
{
    private readonly IUserInfoService _userInfoService = userInfoService;
    private readonly IOptionsMonitor<JwtConfiguration> _jwtConfigs = jwtConfigs;
    private readonly ICacheService _cacheService = cacheService;

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
    
        return await Task.FromResult(new RefreshToken()
        {
            Token = newToken,
            UserId = userId,
            ExpiryDateUtc = tokenExpiry
        });
    }  
}
