using MediatR;

namespace Core.Events.Account;

public record RegularUserCreatedIntegrationEvent(Guid UserId, string? PhoneNumber) : INotification;