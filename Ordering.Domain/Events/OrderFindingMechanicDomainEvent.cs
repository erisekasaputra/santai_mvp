using MediatR; 

namespace Ordering.Domain.Events;

public record OrderFindingMechanicDomainEvent(
    Guid OrderId,
    Guid BuyerId, 
    string BuyerName,
    string BuyerImageUrl,
    double Latitude, 
    double Longitude) : INotification;