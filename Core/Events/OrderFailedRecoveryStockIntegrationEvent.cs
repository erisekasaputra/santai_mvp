using MediatR;

namespace Core.Events;

public record OrderFailedRecoveryStockIntegrationEvent(
    IEnumerable<CatalogItemStockIntegrationEvent> Items) : INotification;
