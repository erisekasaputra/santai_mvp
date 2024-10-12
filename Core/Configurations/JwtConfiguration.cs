namespace Core.Configurations;

public class JwtConfiguration
{
    public const string SectionName = "Jwt";
    public string SecretKey { get; set; } = string.Empty;
    public string Issuer { get; set; } = string.Empty;
    public string Audience { get; set; } = string.Empty;
    public int TotalSecondsAccessTokenLifetime { get; set; }
    public int TotalDaysRefreshTokenLifetime { get; set; } 
    public bool RequireExpirationTime { get; set; }
    public bool ValidateIssuer { get; set; }
    public bool ValidateAudience { get; set; }
    public bool ValidateLifetime { get; set; }
    public bool ValidateIssuerSigningKey { get; set; }  
}
