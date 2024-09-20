using MediatR;

namespace Core.Events.Account;

public record MechanicUserCreatedIntegrationEvent(Guid UserId, string? PhoneNumber) : INotification;