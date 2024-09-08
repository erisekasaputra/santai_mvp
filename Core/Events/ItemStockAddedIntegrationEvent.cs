using MediatR;

namespace Core.Events;

public record ItemStockAddedIntegrationEvent(Guid Id, int Quantity) : INotification;
