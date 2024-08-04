using MediatR;

namespace Catalog.Contracts;

public record ItemPriceSetIntegrationEvent(string Id, decimal Amount) : INotification;
