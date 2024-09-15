using MediatR;

namespace Account.Domain.Events;

public record AccountMechanicOrderAcceptedDomainEvent(Guid OrderId, Guid MechanicId) : INotification;
