using Core.Enumerations;
using MediatR;

namespace Core.Events;

public record ItemPriceSetIntegrationEvent(
    Guid Id,
    decimal OldAmount,
    decimal NewAmount, 
    Currency Currency) : INotification;
