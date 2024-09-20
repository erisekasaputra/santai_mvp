using MediatR;

namespace Core.Events.Ordering;

public record OrderRatedIntegrationEvent(
    Guid OrderId,
    Guid BuyerId,
    decimal Rating,
    string? Comment) : INotification;
