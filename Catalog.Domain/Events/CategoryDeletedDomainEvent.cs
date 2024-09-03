
using MediatR;

namespace Catalog.Domain.Events;

public record CategoryDeletedDomainEvent(Guid Id) : INotification;