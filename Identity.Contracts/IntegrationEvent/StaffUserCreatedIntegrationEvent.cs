using MediatR;

namespace Identity.Contracts.IntegrationEvent;

public record StaffUserCreatedIntegrationEvent(Guid UserId, string? PhoneNumber) : INotification;