using MediatR;

namespace Account.Domain.Events;

public record AccountMechanicAutoSelectedToAnOrderDomainEvent(Guid OrderId, Guid BuyerId, Guid MechanicId) : INotification;