using MediatR;

namespace Core.Events.Account;

public record MechanicDispatchedIntegrationEvent(
    Guid OrderId,
    Guid BuyerId,
    Guid MechanicId,
    string MechanicName) : INotification;
