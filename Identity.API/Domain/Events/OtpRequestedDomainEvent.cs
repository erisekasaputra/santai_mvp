using MediatR;

namespace Identity.API.Domain.Events;

public class OtpRequestedDomainEvent : INotification
{
    public string PhoneNumber { get; set; }
    public string Token { get; set; }

    public OtpRequestedDomainEvent(string phoneNumber, string token)
    {
        PhoneNumber = phoneNumber;
        Token = token;
    }
}
