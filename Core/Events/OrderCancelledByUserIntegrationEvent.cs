using MediatR;

namespace Core.Events;

public record OrderCancelledByUserIntegrationEvent(Guid OrderId, Guid UserId) : INotification; 
