using MediatR;

namespace Core.Events;

public record ItemSoldSetIntegrationEvent(Guid Id, int
    Quantity) : INotification;
