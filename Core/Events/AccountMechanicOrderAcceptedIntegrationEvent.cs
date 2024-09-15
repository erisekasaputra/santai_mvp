
using MediatR;

namespace Core.Events;

public record AccountMechanicOrderAcceptedIntegrationEvent(Guid OrderId, Guid MechanicId) : INotification;
