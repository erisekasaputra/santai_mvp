using MediatR;

namespace Core.Events.Catalog;

public record ItemSoldReducedIntegrationEvent(Guid Id, int Quantity) : INotification;
