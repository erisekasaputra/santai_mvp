using MediatR; 

namespace Ordering.Domain.Events;

public record OrderRatedDomainEvent(
    Guid OrderId,
    Guid BuyerId,
    Guid MechanicId,
    decimal Value,
    string? Comment) : INotification;
