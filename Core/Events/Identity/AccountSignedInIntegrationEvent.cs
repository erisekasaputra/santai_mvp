using MediatR;

namespace Core.Events.Identity;

public record AccountSignedInIntegrationEvent(Guid UserId, string DeviceId) : INotification;
