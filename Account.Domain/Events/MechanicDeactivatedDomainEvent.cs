using Account.Domain.Aggregates.OrderTaskAggregate; 
using MediatR;

namespace Account.Domain.Events;

public record MechanicDeactivatedDomainEvent(MechanicOrderTask User) : INotification;
