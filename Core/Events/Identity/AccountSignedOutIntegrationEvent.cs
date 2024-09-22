using MediatR;

namespace Core.Events.Identity;

public record AccountSignedOutIntegrationEvent(Guid UserId, string DeviceId) : INotification;
