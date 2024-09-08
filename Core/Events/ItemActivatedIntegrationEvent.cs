using MediatR;

namespace Core.Events;

public record ItemActivatedIntegrationEvent(Guid Id) : INotification;
