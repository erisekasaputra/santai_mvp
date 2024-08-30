using Identity.Contracts.Enumerations;
using MediatR;

namespace Identity.Contracts.IntegrationEvent;

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
