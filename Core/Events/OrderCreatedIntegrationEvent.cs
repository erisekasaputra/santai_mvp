using MediatR;

namespace Core.Events;

public record OrderCreatedIntegrationEvent : INotification;
