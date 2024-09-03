using MediatR;

namespace Catalog.Domain.Events;

public record ItemDeletedDomainEvent(Guid Id) : INotification;
