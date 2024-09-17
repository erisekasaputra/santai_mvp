using Core.Enumerations;
using MediatR;
using Ordering.Domain.Aggregates.OrderAggregate;

namespace Ordering.Domain.Events;

public record OrderPaymentPaidDomainEvent(Guid OrderId, Guid BuyerId, decimal Amount, Currency Currency) : INotification;
