
using Core.Configurations;
using Identity.API.Service.Interfaces;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Identity.API.Service;

public class JwtTokenValidator : IJwtTokenValidator
{ 
    private readonly string _issuer;
    private readonly string _audience;
    private readonly string _key;

    public JwtTokenValidator(IOptionsMonitor<JwtConfiguration> jwtConfigs)
    { 
        _issuer = jwtConfigs.CurrentValue.Issuer;
        _audience = jwtConfigs.CurrentValue.Audience;
        _key = jwtConfigs.CurrentValue.SecretKey; 
    }

    public async Task<ClaimsPrincipal> ValidateAsync(string token)
    {
        var key = Encoding.UTF8.GetBytes(_key);

        var tokenHandler = new JwtSecurityTokenHandler();
        var validationParameters = new TokenValidationParameters
        {
            RequireExpirationTime = true,
            ValidateIssuer = true,
            ValidIssuer = _issuer,
            ValidateAudience = true,
            ValidAudience = _audience,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(key),
            ClockSkew = TimeSpan.FromSeconds(0),
        };

        try
        {
            var principal = tokenHandler.ValidateToken(token, validationParameters, out var validatedToken);
            return await Task.FromResult(principal);
        }
        catch (Exception ex)
        { 
            throw new SecurityTokenException("Invalid token", ex);
        }
    } 
}