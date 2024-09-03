using MediatR;

namespace Catalog.Domain.Events;

public record BrandDeletedDomainEvent(Guid Id) : INotification; 
