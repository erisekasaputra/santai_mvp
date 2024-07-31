using MediatR;

namespace Catalog.Domain.Events;

public record ItemSoldSetDomainEvent(string Id, int Quantity) : INotification;