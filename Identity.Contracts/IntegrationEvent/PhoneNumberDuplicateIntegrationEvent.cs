using Identity.Contracts.Entity;

namespace Identity.Contracts.IntegrationEvent;

public class PhoneNumberDuplicateIntegrationEvent
{
    public IEnumerable<DuplicateUser> DuplicateUsers { get; set; }

    public PhoneNumberDuplicateIntegrationEvent(IEnumerable<DuplicateUser> duplicateUsers)
    {
        DuplicateUsers = duplicateUsers;
    } 
}
