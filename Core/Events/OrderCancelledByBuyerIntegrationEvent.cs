using MediatR;

namespace Core.Events;

public record OrderCancelledByBuyerIntegrationEvent : INotification;
