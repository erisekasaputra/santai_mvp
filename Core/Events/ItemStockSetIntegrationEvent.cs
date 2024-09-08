using MediatR;

namespace Core.Events;

public record ItemStockSetIntegrationEvent(Guid Id, int Quantity) : INotification;
