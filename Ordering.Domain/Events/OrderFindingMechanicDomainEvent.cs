using MediatR; 

namespace Ordering.Domain.Events;

public record OrderFindingMechanicDomainEvent(
    Guid OrderId,
    Guid BuyerId, 
    double Latitude, 
    double Longitude) : INotification;