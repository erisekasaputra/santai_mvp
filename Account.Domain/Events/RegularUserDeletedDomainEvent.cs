using MediatR;

namespace Account.Domain.Events;

public record RegularUserDeletedDomainEvent(Guid Id) : INotification;
