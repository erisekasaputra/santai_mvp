using MediatR;

namespace Core.Events;

public record ServiceCompletedIntegrationEvent(
    Guid OrderId,
    Guid BuyerId,
    Guid MechanicId) : INotification;
