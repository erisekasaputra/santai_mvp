using Account.Domain.Aggregates.UserAggregate;
using MediatR;

namespace Account.Domain.Events;

public record MechanicDeactivatedDomainEvent(MechanicUser User) : INotification;
