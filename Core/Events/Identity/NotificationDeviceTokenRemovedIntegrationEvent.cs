using MediatR;

namespace Core.Events.Identity;

public record NotificationDeviceTokenRemovedIntegrationEvent(Guid UserId, string DeviceId) : INotification;