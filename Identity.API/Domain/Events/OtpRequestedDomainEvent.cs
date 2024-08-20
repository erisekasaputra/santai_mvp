using MediatR;

namespace Identity.API.Domain.Events;

public class OtpRequestedDomainEvent : INotification
{
    public string Address { get; set; }
    public string Token { get; set; } 
    public string Provider { get; set; }

    public OtpRequestedDomainEvent(string address, string token, string provider)
    {
        Address = address ?? throw new Exception("Address must not empty");
        Token = token ?? throw new Exception("Token must not empty");
        Provider = provider ?? throw new Exception("Provider must not empty");
    }
}
