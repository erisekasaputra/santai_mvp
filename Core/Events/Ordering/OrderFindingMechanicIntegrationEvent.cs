using MediatR;

namespace Core.Events.Ordering;

public record OrderFindingMechanicIntegrationEvent(
    Guid OrderId,
    Guid BuyerId,
    string BuyerName,
    string BuyerImageUrl,
    double Latitude,
    double Longitude) : INotification;
