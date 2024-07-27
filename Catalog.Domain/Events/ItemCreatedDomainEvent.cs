using Catalog.Domain.Aggregates.ItemAggregate;
using MediatR;

namespace Catalog.Domain.Events;

public record ItemCreatedDomainEvent(Item Item) : INotification;
