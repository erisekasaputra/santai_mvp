namespace Identity.Contracts;

public class IdentityPhoneNumberConfirmedIntegrationEvent
{
    public Guid Id { get; set; }
    public string PhoneNumber { get; set; }

    public IdentityPhoneNumberConfirmedIntegrationEvent(Guid id, string phoneNumber)
    {
        Id = id;
        PhoneNumber = phoneNumber;
    }
}
