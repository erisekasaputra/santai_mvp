using MediatR;

namespace Core.Events;

public record ItemUndeletedIntegrationEvent(Guid Id) : INotification;
