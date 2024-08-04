using MediatR;

namespace Catalog.Contracts;

public record ItemStockReducedIntegrationEvent(string Id, int Quantity) : INotification;
