using MediatR;

namespace Core.Events.Account;

public record MechanicAutoSelectedIntegrationEvent(
    Guid OrderId,
    Guid BuyerId,
    Guid MechanicId,
    string MechanicName,
    string MechanicImageUrl,
    int ConfirmDeadlineInSeconds) : INotification;
