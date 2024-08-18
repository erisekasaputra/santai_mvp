namespace Identity.Contracts;

public class OtpRequestedIntegrationEvent
{
    public string PhoneNumber { get; set; }
    public string Token { get; set; }

    public OtpRequestedIntegrationEvent(string phoneNumber, string token)
    {
        PhoneNumber = phoneNumber;
        Token = token;
    }
}
