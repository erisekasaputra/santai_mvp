using MediatR;

namespace Catalog.Domain.Events;

public record ItemSoldSetDomainEvent(Guid Id, int Quantity) : INotification;