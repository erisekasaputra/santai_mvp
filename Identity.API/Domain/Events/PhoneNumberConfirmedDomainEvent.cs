using Core.Enumerations; 
using MediatR;

namespace Identity.API.Domain.Events;

public class PhoneNumberConfirmedDomainEvent : INotification
{
    public Guid Sub { get; set; }
    public string PhoneNumber { get; set; } = string.Empty; 
    public UserType UserType { get; set; }

    public PhoneNumberConfirmedDomainEvent(Guid id, string phoneNumber, UserType userType)
    {
        Sub = id;
        PhoneNumber = phoneNumber;
        UserType = userType;    
    }
}
