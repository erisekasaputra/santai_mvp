using Account.Domain.Aggregates.UserAggregate;
using MediatR;

namespace Account.Domain.Events;

public record RegularUserCreateDomainEvent(RegularUser user) : INotification;
