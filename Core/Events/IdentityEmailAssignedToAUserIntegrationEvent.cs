using Core.Enumerations; 
using MediatR;

namespace Core.Events;

public class IdentityEmailAssignedToAUserIntegrationEvent : INotification
{
    public Guid Id { get; set; }
    public string Email { get; set; }
    public UserType UserType { get; set; }

    public IdentityEmailAssignedToAUserIntegrationEvent(Guid id, string email, UserType userType)
    {
        Id = id;
        Email = email;
        UserType = userType;
    }
}
