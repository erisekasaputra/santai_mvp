using MediatR;

namespace Core.Events;

public record OrderFindingMechanicIntegrationEvent(
    Guid OrderId, 
    Guid BuyerId,
    double Latitude, 
    double Longitude) : INotification;
