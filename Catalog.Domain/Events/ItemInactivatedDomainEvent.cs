using MediatR;

namespace Catalog.Domain.Events;

public record ItemInactivatedDomainEvent(Guid Id) : INotification;
