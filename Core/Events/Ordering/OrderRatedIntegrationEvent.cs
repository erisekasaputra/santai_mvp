using MediatR;

namespace Core.Events.Ordering;

public record OrderRatedIntegrationEvent(
    Guid OrderId,
    Guid BuyerId,
    Guid MechanicId,
    decimal Rating,
    string? Comment) : INotification;
