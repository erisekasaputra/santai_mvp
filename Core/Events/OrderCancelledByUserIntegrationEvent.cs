using MediatR;

namespace Core.Events;

public record OrderCancelledByUserIntegrationEvent : INotification; 
