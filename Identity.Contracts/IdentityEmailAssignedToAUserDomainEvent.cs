namespace Identity.Contracts;

public class IdentityEmailAssignedToAUserDomainEvent
{
    public Guid Id { get; set; }
    public string Email { get; set; }

    public IdentityEmailAssignedToAUserDomainEvent(Guid id, string email)
    {
        Id = id;
        Email = email;
    }
}
