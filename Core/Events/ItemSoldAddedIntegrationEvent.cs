using MediatR;

namespace Core.Events;

public record ItemSoldAddedIntegrationEvent(Guid Id, int Quantity) : INotification;
