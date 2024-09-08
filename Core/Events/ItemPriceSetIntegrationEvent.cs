using MediatR;

namespace Core.Events;

public record ItemPriceSetIntegrationEvent(Guid Id, decimal Amount) : INotification;
