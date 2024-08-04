using MediatR;

namespace Catalog.Domain.Events;

public record ItemInactivatedDomainEvent(string Id) : INotification;
