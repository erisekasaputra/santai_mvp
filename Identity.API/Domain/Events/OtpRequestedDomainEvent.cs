using Identity.Contracts.Enumerations;
using MediatR;

namespace Identity.API.Domain.Events;

public class OtpRequestedDomainEvent : INotification
{
    public string PhoneNumber { get; set; }
    public string? Email { get; set; }
    public string Token { get; set; } 
    public OtpProviderType Provider { get; set; }

    public OtpRequestedDomainEvent(string phoneNumber, string? email, string token, OtpProviderType provider)
    {
        PhoneNumber = phoneNumber ?? throw new Exception("Phone number can not be empty");
        Email = email;
        Token = token ?? throw new Exception("Token must can not be empty");
        Provider = provider;
    }
}
