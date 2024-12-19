using MediatR;

namespace Account.Domain.Events;

public record AccountMechanicOrderAcceptedDomainEvent(
    Guid OrderId, 
    Guid BuyerId, 
    Guid MechanicId,
    string MechanicName,
    string MechanicImageUrl,
    decimal Performance
    ) : INotification;
