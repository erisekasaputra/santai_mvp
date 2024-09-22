using MediatR;

namespace Identity.API.Domain.Events;

public record AccountSignedOutDomainEvent(Guid UserId, string DeviceId) : INotification;
