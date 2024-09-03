using MediatR;

namespace Catalog.Domain.Events;

public record ItemPriceSetDomainEvent(Guid Id, decimal Amount) : INotification;
