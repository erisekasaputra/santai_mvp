using MediatR;

namespace Core.Events.Catalog;

public record CategoryDeletedIntegrationEvent(Guid Id) : INotification;
