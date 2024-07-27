using MediatR;

namespace Catalog.Domain.Events;

public record ItemDeletedDomainEvent(string Id) : INotification;
