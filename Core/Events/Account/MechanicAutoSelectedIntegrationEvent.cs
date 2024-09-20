using MediatR;

namespace Core.Events.Account;

public record MechanicAutoSelectedIntegrationEvent(
    Guid OrderId,
    Guid BuyerId,
    Guid MechanicId) : INotification;
