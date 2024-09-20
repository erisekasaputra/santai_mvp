using MediatR;

namespace Core.Events.Ordering;

public record OrderRejectedByMechanicIntegrationEvent : INotification;
