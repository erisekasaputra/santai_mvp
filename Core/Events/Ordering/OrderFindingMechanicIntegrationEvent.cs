using MediatR;

namespace Core.Events.Ordering;

public record OrderFindingMechanicIntegrationEvent(
    Guid OrderId,
    Guid BuyerId,
    double Latitude,
    double Longitude) : INotification;
