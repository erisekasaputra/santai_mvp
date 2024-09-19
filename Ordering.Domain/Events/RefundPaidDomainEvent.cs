using Core.Enumerations;
using MediatR;

namespace Ordering.Domain.Events;

public record RefundPaidDomainEvent(
    Guid OrderId, 
    Guid BuyerId, 
    decimal Amount,
    Currency Currency) : INotification;
