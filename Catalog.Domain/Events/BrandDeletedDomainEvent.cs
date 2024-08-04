using MediatR;

namespace Catalog.Domain.Events;

public record BrandDeletedDomainEvent(string Id) : INotification; 
