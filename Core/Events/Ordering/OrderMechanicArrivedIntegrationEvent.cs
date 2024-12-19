using MediatR;

namespace Core.Events.Ordering;

public record OrderMechanicArrivedIntegrationEvent(
    Guid OrderId,
    Guid BuyerId,
    Guid MechanicId,
    string MechanicName ) : INotification;
