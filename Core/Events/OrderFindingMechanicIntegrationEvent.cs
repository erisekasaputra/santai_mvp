using MediatR;

namespace Core.Events;

public record OrderFindingMechanicIntegrationEvent(
    Guid OrderId, 
    double Latitude, 
    double Longitude) : INotification;
