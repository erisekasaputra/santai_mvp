using MediatR;

namespace Identity.Contracts.IntegrationEvent;

public record MechanicUserCreatedIntegrationEvent(Guid UserId, string? PhoneNumber) : INotification;