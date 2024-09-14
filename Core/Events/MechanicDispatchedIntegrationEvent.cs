using MediatR;

namespace Core.Events;

public record MechanicDispatchedIntegrationEvent(
    Guid OrderId,
    Guid BuyerId,
    Guid MechanicId) : INotification;
