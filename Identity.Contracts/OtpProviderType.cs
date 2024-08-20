namespace Identity.Contracts;

public class OtpProviderType
{ 
    public const string SMS = "sms";
    public const string Whatsapp = "whatsapp";
    public const string Email = "email";
    private static List<string> _allowedProviderType = [SMS, Whatsapp, Email];
    public static IReadOnlyList<string> AllowedProviderType => _allowedProviderType.AsReadOnly();
}
