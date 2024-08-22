using Identity.Contracts.Enumerations;

namespace Identity.Contracts;

public class OtpRequestedIntegrationEvent
{
    public string PhoneNumber { get; set; }
    public string Token { get; set; }
    public OtpProviderType Provider { get; set; }

    public OtpRequestedIntegrationEvent(string phoneNumber, string token, OtpProviderType provider)
    {
        PhoneNumber = phoneNumber;
        Token = token;
        Provider = provider;    
    }  
}
