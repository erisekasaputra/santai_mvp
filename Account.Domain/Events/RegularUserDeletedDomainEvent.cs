using MediatR;

namespace Account.Domain.Events;

public record RegularUserDeletedDomainEvent(Guid id) : INotification;
