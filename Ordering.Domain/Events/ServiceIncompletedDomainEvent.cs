using MediatR; 

namespace Ordering.Domain.Events;

public record ServiceIncompletedDomainEvent(
    Guid OrderId,
    Guid BuyerId,
    Guid MechanicId,
    string MechanicName) : INotification;
