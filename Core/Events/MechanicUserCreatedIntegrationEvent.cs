using MediatR;

namespace Core.Events;

public record MechanicUserCreatedIntegrationEvent(Guid UserId, string? PhoneNumber) : INotification;