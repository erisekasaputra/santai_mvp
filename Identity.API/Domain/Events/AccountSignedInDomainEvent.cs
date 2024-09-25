using MediatR;

namespace Identity.API.Domain.Events;

public record AccountSignedInDomainEvent(
    Guid UserId, 
    string DeviceId,
    string PhoneNumber,
    string? Email) : INotification;
