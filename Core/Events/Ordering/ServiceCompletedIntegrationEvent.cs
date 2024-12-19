using MediatR;

namespace Core.Events.Ordering;

public record ServiceCompletedIntegrationEvent(
    Guid OrderId,
    Guid BuyerId,
    Guid MechanicId,
    string MechanicName ) : INotification;
