using Core.Enumerations; 
using MediatR;

namespace Identity.API.Domain.Events;

public class EmailAssignedToAUserDomainEvent : INotification
{
    public Guid Sub { get; set; }
    public string Email { get; set; } = string.Empty;
    public UserType UserType { get; set; }

    public EmailAssignedToAUserDomainEvent(Guid id, string email, UserType userType)
    {
        Sub = id;
        Email = email;
        UserType = userType;
    }
}
