using MediatR;

namespace Catalog.Contracts;

public record ItemStockSetIntegrationEvent(Guid Id, int Quantity) : INotification;
