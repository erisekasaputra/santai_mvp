using MediatR;

namespace Catalog.Domain.Events;

public record ItemStockAddedDomainEvent(Guid Id, int Quantity) : INotification;
