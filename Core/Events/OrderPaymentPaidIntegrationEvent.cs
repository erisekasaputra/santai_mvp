using Core.Enumerations;
using MediatR;

namespace Core.Events;

public record OrderPaymentPaidIntegrationEvent(
    Guid OrderId,
    decimal Amount,
    Currency Currency) : INotification;
