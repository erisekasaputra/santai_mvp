using MediatR;

namespace Catalog.Domain.Events;

public record ItemStockReducedDomainEvent(Guid Id, int Quantity) : INotification;
