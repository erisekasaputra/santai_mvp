using MediatR;

namespace Identity.Contracts.IntegrationEvent;

public record RegularUserCreatedIntegrationEvent(Guid UserId, string? PhoneNumber) : INotification;