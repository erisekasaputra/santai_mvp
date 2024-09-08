using MediatR;

namespace Core.Events;

public record ItemStockReducedIntegrationEvent(Guid Id, int Quantity) : INotification;
