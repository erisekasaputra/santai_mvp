using Identity.Contracts.Enumerations;
using MediatR;

namespace Identity.API.Domain.Events;

public class OtpRequestedDomainEvent : INotification
{
    public string Address { get; set; }
    public string Token { get; set; } 
    public OtpProviderType Provider { get; set; }

    public OtpRequestedDomainEvent(string address, string token, OtpProviderType provider)
    {
        Address = address ?? throw new Exception("Address must not empty");
        Token = token ?? throw new Exception("Token must not empty");
        Provider = provider;
    }
}
