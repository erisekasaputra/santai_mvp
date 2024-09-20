using MediatR;

namespace Core.Events.Account;

public record StaffUserDeletedIntegrationEvent(Guid UserId) : INotification;
