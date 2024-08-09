namespace Account.API.Options;

public class RateLimiterRule
{
    public string Endpoint { get; set; } = string.Empty;
    public string Period { get; set; } = string.Empty;
    public string Limit { get; set; } = string.Empty;
} 