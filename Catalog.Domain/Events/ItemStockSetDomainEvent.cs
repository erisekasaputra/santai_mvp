using MediatR;

namespace Catalog.Domain.Events;

public record ItemStockSetDomainEvent(string Id, int Quantity) : INotification;
