using MediatR;

namespace Catalog.Contracts;

public record ItemSoldAddedIntegrationEvent(Guid Id, int Quantity) : INotification;
