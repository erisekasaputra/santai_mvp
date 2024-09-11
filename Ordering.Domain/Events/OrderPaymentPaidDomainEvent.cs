using MediatR;
using Ordering.Domain.Aggregates.OrderAggregate;

namespace Ordering.Domain.Events;

public record OrderPaymentPaidDomainEvent(Order Order) : INotification;
