using MediatR;

namespace Core.Events.Catalog;

public record ItemSoldSetIntegrationEvent(Guid Id, int
    Quantity) : INotification;
