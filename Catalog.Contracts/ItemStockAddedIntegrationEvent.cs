using MediatR;

namespace Catalog.Contracts;

public record ItemStockAddedIntegrationEvent(Guid Id, int Quantity) : INotification;
