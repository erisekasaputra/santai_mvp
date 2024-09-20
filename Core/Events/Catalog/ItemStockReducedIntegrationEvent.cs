using MediatR;

namespace Core.Events.Catalog;

public record ItemStockReducedIntegrationEvent(Guid Id, int Quantity) : INotification;
