using Core.Enumerations;
using MediatR;

namespace Core.Events;

public record ItemPriceSetIntegrationEvent(Guid Id, decimal Amount, Currency Currency) : INotification;
