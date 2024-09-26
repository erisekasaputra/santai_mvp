using MediatR; 

namespace Ordering.Domain.Events;

public record MechanicAssignedDomainEvent(
    Guid OrderId , 
    Guid BuyerId, 
    Guid MechanicId,
    string MechanicName,
    int ConfirmDeadlineInSeconds) : INotification;
