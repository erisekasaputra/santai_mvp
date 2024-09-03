using MediatR;

namespace Catalog.Domain.Events;

public record ItemStockSetDomainEvent(Guid Id, int Quantity) : INotification;
