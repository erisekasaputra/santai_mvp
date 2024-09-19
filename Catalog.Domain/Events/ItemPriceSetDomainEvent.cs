using Core.Enumerations;
using MediatR;

namespace Catalog.Domain.Events;

public record ItemPriceSetDomainEvent(
    Guid Id, 
    decimal OldPrice,
    decimal NewAmount, 
    Currency Currency) : INotification;
