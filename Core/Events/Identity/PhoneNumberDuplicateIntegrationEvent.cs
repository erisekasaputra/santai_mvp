using Core.Models;
using MediatR;

namespace Core.Events.Identity;

public class PhoneNumberDuplicateIntegrationEvent : INotification
{
    public IEnumerable<DuplicateUser> DuplicateUsers { get; set; }
    public PhoneNumberDuplicateIntegrationEvent(IEnumerable<DuplicateUser> duplicateUsers)
    {
        DuplicateUsers = duplicateUsers;
    }
}
