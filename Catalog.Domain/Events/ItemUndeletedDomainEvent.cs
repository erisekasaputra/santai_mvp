using MediatR;

namespace Catalog.Domain.Events;

public record ItemUndeletedDomainEvent(Guid Id) : INotification;
