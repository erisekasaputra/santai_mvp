using Catalog.Domain.Aggregates.BrandAggregate;
using MediatR;

namespace Catalog.Domain.Events;

public record BrandUpdatedDomainEvent(Brand Brand) : INotification;
