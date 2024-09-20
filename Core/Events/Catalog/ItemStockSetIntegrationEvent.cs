using MediatR;

namespace Core.Events.Catalog;

public record ItemStockSetIntegrationEvent(Guid Id, int Quantity) : INotification;
