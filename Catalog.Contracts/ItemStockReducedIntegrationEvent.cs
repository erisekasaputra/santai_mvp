using MediatR;

namespace Catalog.Contracts;

public record ItemStockReducedIntegrationEvent(Guid Id, int Quantity) : INotification;
