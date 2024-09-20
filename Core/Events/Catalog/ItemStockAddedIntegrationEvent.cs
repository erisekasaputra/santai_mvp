using MediatR;

namespace Core.Events.Catalog;

public record ItemStockAddedIntegrationEvent(Guid Id, int Quantity) : INotification;
