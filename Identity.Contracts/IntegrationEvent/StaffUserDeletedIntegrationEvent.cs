using MediatR;

namespace Identity.Contracts.IntegrationEvent;

public record StaffUserDeletedIntegrationEvent(Guid UserId) : INotification;
