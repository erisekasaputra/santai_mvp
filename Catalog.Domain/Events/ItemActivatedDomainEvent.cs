using MediatR;

namespace Catalog.Domain.Events;

public record ItemActivatedDomainEvent(string Id) : INotification;
