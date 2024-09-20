using MediatR;

namespace Core.Events.Account;

public record RegularUserDeletedIntegrationEvent(Guid UserId) : INotification;