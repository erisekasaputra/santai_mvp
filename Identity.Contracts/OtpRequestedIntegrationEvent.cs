namespace Identity.Contracts;

public class OtpRequestedIntegrationEvent
{
    public string PhoneNumber { get; set; }
    public string Token { get; set; }
    public string Provider { get; set; }

    public OtpRequestedIntegrationEvent(string phoneNumber, string token, string provider)
    {
        PhoneNumber = phoneNumber;
        Token = token;
        Provider = provider;    
    }
}
