namespace Core.Authentications;

public class AuthenticationClient
{
    public AuthenticationClientScheme AuthenticationScheme { get; set; }
    public List<AuthenticationPolicy> Policies { get; set; } = [];
    public string Issuer { get; set; } = string.Empty;
    public string Audience { get; set; } = string.Empty;
}
