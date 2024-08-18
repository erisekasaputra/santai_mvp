using Identity.API.Abstraction;
using Identity.API.Configs;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text.Json;

namespace Identity.API.Service;

public class GoogleTokenValidator : IGoogleTokenValidator
{
    private readonly string _googlePublicKeysUrl = "https://www.googleapis.com/oauth2/v3/certs";

    private readonly string _audience;
    private readonly string _issuer;
    public GoogleTokenValidator(IOptionsMonitor<GoogleConfig> googleConfigs)
    {
        _audience = googleConfigs.CurrentValue.Audience;
        _issuer = googleConfigs.CurrentValue.Issuer;
    }

    public async Task<ClaimsPrincipal> ValidateAsync(string token)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
         
        if (tokenHandler.ReadJwtToken(token) is null)
        {
            throw new SecurityTokenException("Invalid jwt token");
        }
         
        var httpClient = new HttpClient();
        var publicKeysResponse = await httpClient.GetStringAsync(_googlePublicKeysUrl);
        var publicKeys = JsonDocument.Parse(publicKeysResponse).RootElement.GetProperty("keys");

        var tokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuers = [_issuer , _issuer.TrimEnd('/')],
            ValidateAudience = true,
            ValidAudience = _audience,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            IssuerSigningKeys = GetSigningKeys(publicKeys),
            RequireExpirationTime = true
        };

        try
        {
            var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out var validatedToken);
            return principal;
        }
        catch (Exception ex)
        {
            // Handle validation errors
            throw new SecurityTokenInvalidSignatureException("Invalid token signature", ex);
        }
    }

    private IEnumerable<SecurityKey> GetSigningKeys(JsonElement publicKeys)
    {
        var signingKeys = new List<SecurityKey>();

        foreach (var key in publicKeys.EnumerateArray())
        {
            if (key.TryGetProperty("kid", out var kid) &&
                key.TryGetProperty("kty", out var kty) &&
                key.TryGetProperty("n", out var n) &&
                key.TryGetProperty("e", out var e))
            {
                var rsaParameters = new RSAParameters
                {
                    Modulus = Base64UrlEncoder.DecodeBytes(n.GetString()),
                    Exponent = Base64UrlEncoder.DecodeBytes(e.GetString())
                };

                var rsa = new RsaSecurityKey(rsaParameters) { KeyId = kid.GetString() };
                signingKeys.Add(rsa);
            }
        }

        return signingKeys;
    }
}
