using MediatR;

namespace Core.Events;

public record ServiceProcessedIntegrationEvent(
    Guid OrderId,
    Guid BuyerId,
    Guid MechanicId) : INotification;
