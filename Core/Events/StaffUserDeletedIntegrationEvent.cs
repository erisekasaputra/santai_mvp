using MediatR;

namespace Core.Events;

public record StaffUserDeletedIntegrationEvent(Guid UserId) : INotification;
