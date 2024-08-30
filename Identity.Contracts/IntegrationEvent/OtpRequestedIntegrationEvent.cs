using Identity.Contracts.Enumerations;
using MediatR;

namespace Identity.Contracts.IntegrationEvent;

public class OtpRequestedIntegrationEvent : INotification
{
    public string PhoneNumber { get; set; }
    public string? Email { get; set; }
    public string Token { get; set; }
    public OtpProviderType Provider { get; set; }

    public OtpRequestedIntegrationEvent(string phoneNumber, string? email, string token, OtpProviderType provider)
    {
        PhoneNumber = phoneNumber;
        Email = email;
        Token = token;
        Provider = provider;
    }
}
