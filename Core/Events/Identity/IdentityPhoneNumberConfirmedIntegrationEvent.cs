using Core.Enumerations;
using MediatR;

namespace Core.Events.Identity;

public class IdentityPhoneNumberConfirmedIntegrationEvent : INotification
{
    public Guid Id { get; set; }
    public string PhoneNumber { get; set; }
    public UserType UserType { get; set; }

    public IdentityPhoneNumberConfirmedIntegrationEvent(Guid id, string phoneNumber, UserType userType)
    {
        Id = id;
        PhoneNumber = phoneNumber;
        UserType = userType;
    }
}
