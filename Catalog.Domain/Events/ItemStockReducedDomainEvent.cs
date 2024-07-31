using MediatR;

namespace Catalog.Domain.Events;

public record ItemStockReducedDomainEvent(string Id, int Quantity) : INotification;
