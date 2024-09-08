using MediatR;

namespace Core.Events;

public record ItemSoldReducedIntegrationEvent(Guid Id, int Quantity) : INotification;
