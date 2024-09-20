using MediatR;

namespace Core.Events.Catalog;

public record ItemActivatedIntegrationEvent(Guid Id) : INotification;
