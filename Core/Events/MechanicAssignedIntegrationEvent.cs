using MediatR;

namespace Core.Events;

public record MechanicAssignedIntegrationEvent(
    Guid OrderId,
    Guid BuyerId,
    Guid MechanicId) : INotification;
