using Core.Enumerations;
using MediatR;

namespace Core.Events.Ordering;

public record OrderPaymentPaidIntegrationEvent(
    Guid OrderId,
    Guid BuyerId,
    decimal Amount,
    Currency Currency) : INotification;
