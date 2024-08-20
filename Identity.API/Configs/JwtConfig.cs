namespace Identity.API.Configs;

public class JwtConfig
{
    public const string SectionName = "Jwt";
    public string SecretKey { get; set; } = string.Empty;
    public string Issuer { get; set; } = string.Empty;
    public string Audience { get; set; } = string.Empty;

    public int TotalSecondsAccessTokenLifetime { get; set; }
    public int TotalDaysRefreshTokenLifetime { get; set; }
}
