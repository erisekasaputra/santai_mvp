using MediatR;

namespace Catalog.Domain.Events;

public record ItemPriceSetDomainEvent(string Id, decimal Amount) : INotification;
