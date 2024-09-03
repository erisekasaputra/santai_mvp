using MediatR;

namespace Catalog.Domain.Events;

public record ItemActivatedDomainEvent(Guid Id) : INotification;
