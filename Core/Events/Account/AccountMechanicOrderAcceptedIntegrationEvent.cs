using MediatR;

namespace Core.Events.Account;

public record AccountMechanicOrderAcceptedIntegrationEvent(
    Guid OrderId, 
    Guid BuyerId, 
    Guid MechanicId,
    string MechanicName,
    string MechanicImageUrl,
    decimal Performance
   ) : INotification;
