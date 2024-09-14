using MediatR;

namespace Core.Events;

public record OrderRatedIntegrationEvent(
    Guid OrderId,
    Guid BuyerId,
    decimal Rating,
    string? Comment) : INotification;
