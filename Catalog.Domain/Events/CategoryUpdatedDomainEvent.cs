using Catalog.Domain.Aggregates.CategoryAggregate;
using MediatR;

namespace Catalog.Domain.Events;

public record CategoryUpdatedDomainEvent(Category Category) : INotification;
