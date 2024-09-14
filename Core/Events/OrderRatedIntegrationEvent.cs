using MediatR;

namespace Core.Events;

public record OrderRatedIntegrationEvent(
    Guid OrderId,
    decimal Rating,
    string? Comment) : INotification;
