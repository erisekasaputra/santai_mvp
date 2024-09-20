using MediatR;

namespace Core.Events.Catalog;

public record ItemInactivatedIntegrationEvent(Guid Id) : INotification;
