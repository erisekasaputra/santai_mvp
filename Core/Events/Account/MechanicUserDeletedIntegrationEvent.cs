using MediatR;

namespace Core.Events.Account;

public record MechanicUserDeletedIntegrationEvent(Guid UserId) : INotification;