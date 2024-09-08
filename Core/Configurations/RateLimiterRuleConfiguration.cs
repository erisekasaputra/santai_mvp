namespace Core.Configurations;

public class RateLimiterRuleConfiguration
{
    public const string SectionName = "RateLimiterRule";
    public string Endpoint { get; set; } = string.Empty;
    public string Period { get; set; } = string.Empty;
    public string Limit { get; set; } = string.Empty;
}