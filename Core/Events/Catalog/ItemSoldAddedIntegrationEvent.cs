using MediatR;

namespace Core.Events.Catalog;

public record ItemSoldAddedIntegrationEvent(Guid Id, int Quantity) : INotification;
