using Account.Domain.Aggregates.OrderTaskAggregate; 
using MediatR;

namespace Account.Domain.Events;

public record MechanicActivatedDomainEvent(MechanicOrderTask User) : INotification;
