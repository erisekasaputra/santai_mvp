namespace Core.Configurations;

public class SenangPayPaymentConfiguration
{
    public const string SectionName = "SenangPay";
    public string Host { get; set; } = string.Empty;
    public string MerchantId { get; set; } = string.Empty;
    public string SecretKey { get; set; } = string.Empty;
    public int TimeoutSeconds { get; set; } = 0;
}
