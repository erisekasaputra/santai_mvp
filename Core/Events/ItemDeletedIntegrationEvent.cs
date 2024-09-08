using MediatR;

namespace Core.Events;

public record ItemDeletedIntegrationEvent(Guid Id) : INotification;
