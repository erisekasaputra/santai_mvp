using MediatR;

namespace Core.Events;

public record OrderRejectedByMechanicIntegrationEvent : INotification;
