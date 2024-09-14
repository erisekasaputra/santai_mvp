using MediatR;

namespace Core.Events;

public record OrderCancelledByMechanicIntegrationEvent(Guid OrderId, Guid MechanicId) : INotification;
