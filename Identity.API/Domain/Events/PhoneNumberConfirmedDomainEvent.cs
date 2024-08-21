using MediatR;

namespace Identity.API.Domain.Events;

public class PhoneNumberConfirmedDomainEvent : INotification
{
    public Guid Sub { get; set; }
    public string PhoneNumber { get; set; } = string.Empty;
    public PhoneNumberConfirmedDomainEvent(Guid id, string phoneNumber)
    {
        Sub = id;
        PhoneNumber = phoneNumber;
    }
}
