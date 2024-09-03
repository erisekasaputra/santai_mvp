using MediatR;

namespace Catalog.Contracts;

public record ItemPriceSetIntegrationEvent(Guid Id, decimal Amount) : INotification;
