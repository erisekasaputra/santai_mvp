using Core.Enumerations;
using MediatR;

namespace Catalog.Domain.Events;

public record ItemPriceSetDomainEvent(Guid Id, decimal Amount, Currency Currency) : INotification;
