namespace Core.Configurations;

public class RateLimiterConfiguration
{
    public const string SectionName = "RateLimiter";
    public bool EnableRateLimiting { get; set; } = true;
    public bool StackBlockedRequests { get; set; } = true;
    public string RealIPHeader { get; set; } = string.Empty;
    public string ClientIdHeader { get; set; } = string.Empty;
    public List<RateLimiterRuleConfiguration> GeneralRules { get; set; } = [];
}
