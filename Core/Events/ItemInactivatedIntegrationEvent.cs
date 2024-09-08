using MediatR;

namespace Core.Events;

public record ItemInactivatedIntegrationEvent(Guid Id) : INotification;
