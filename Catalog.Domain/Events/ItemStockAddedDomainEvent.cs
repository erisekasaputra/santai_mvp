using MediatR;

namespace Catalog.Domain.Events;

public record ItemStockAddedDomainEvent(string Id, int Quantity) : INotification;
