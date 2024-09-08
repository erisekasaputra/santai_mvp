using MediatR;

namespace Core.Events;

public record RegularUserCreatedIntegrationEvent(Guid UserId, string? PhoneNumber) : INotification;