using MediatR;

namespace Identity.Contracts.IntegrationEvent;

public record RegularUserDeletedIntegrationEvent(Guid UserId) : INotification;