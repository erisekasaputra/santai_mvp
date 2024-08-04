using MediatR;

namespace Catalog.Contracts;

public record ItemStockSetIntegrationEvent(string Id, int Quantity) : INotification;
