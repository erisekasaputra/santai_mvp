using MediatR;

namespace Core.Events;

public record CategoryDeletedIntegrationEvent(Guid Id) : INotification;
