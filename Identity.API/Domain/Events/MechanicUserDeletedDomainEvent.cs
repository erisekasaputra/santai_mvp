using MediatR;

namespace Identity.API.Domain.Events;

public record MechanicUserDeletedDomainEvent(Guid UserId) : INotification;
