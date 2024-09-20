using MediatR;

namespace Core.Events.Catalog;

public record ItemDeletedIntegrationEvent(Guid Id) : INotification;
