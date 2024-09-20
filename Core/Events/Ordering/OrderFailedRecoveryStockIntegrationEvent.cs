using Core.Events.Catalog;
using MediatR;

namespace Core.Events.Ordering;

public record OrderFailedRecoveryStockIntegrationEvent(
    IEnumerable<CatalogItemStockIntegrationEvent> Items) : INotification;
