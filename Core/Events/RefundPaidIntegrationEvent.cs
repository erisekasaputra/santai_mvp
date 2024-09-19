using Core.Enumerations;
using MediatR;

namespace Core.Events;

public record RefundPaidIntegrationEvent(
    Guid OrderId, 
    Guid Buyerid, 
    decimal Amount,
    Currency Currency) : INotification;
