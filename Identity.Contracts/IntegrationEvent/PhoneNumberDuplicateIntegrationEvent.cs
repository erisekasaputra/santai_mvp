using Identity.Contracts.Entity;
using MediatR;

namespace Identity.Contracts.IntegrationEvent;

public class PhoneNumberDuplicateIntegrationEvent : INotification
{
    public IEnumerable<DuplicateUser> DuplicateUsers { get; set; }

    public PhoneNumberDuplicateIntegrationEvent(IEnumerable<DuplicateUser> duplicateUsers)
    {
        DuplicateUsers = duplicateUsers;
    } 
}
