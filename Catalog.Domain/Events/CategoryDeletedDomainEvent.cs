
using MediatR;

namespace Catalog.Domain.Events;

public record CategoryDeletedDomainEvent(string Id) : INotification;