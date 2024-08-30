using Account.Domain.Aggregates.UserAggregate;
using MediatR;

namespace Account.Domain.Events;

public record StaffDeletedDomainEvent(Staff Staff) : INotification;
