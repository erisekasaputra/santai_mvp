using MediatR;

namespace Core.Events.Identity;

public record AccountSignedInIntegrationEvent(
    Guid UserId, 
    string DeviceId,
    string PhoneNumber,
    string? Email) : INotification;
