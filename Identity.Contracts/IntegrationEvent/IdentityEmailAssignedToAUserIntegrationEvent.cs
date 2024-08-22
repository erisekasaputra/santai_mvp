namespace Identity.Contracts.IntegrationEvent;

public class IdentityEmailAssignedToAUserIntegrationEvent
{
    public Guid Id { get; set; }
    public string Email { get; set; }

    public IdentityEmailAssignedToAUserIntegrationEvent(Guid id, string email)
    {
        Id = id;
        Email = email;
    } 
}
