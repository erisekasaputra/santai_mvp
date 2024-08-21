using MediatR;

namespace Identity.API.Domain.Events;

public class EmailAssignedToAUserDomainEvent : INotification
{
    public Guid Sub { get; set; }
    public string Email { get; set; } = string.Empty;

    public EmailAssignedToAUserDomainEvent(Guid id, string email)
    {
        Sub = id;
        Email = email;
    }
}
