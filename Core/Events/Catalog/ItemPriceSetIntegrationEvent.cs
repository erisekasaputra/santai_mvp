using Core.Enumerations;
using MediatR;

namespace Core.Events.Catalog;

public record ItemPriceSetIntegrationEvent(
    Guid Id,
    decimal OldAmount,
    decimal NewAmount,
    Currency Currency) : INotification;
