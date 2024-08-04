using MediatR;

namespace Catalog.Domain.Events;

public record ItemUndeletedDomainEvent(string Id) : INotification;
