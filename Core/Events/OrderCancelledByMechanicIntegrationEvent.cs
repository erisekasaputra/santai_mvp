using MediatR;

namespace Core.Events;

public record OrderCancelledByMechanicIntegrationEvent : INotification;
