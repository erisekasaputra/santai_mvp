using MediatR;

namespace Account.Domain.Events;

public record MechanicAssignedDomainEvent(Guid OrderId, Guid MechanicId) : INotification;
