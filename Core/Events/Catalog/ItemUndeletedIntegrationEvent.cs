using MediatR;

namespace Core.Events.Catalog;

public record ItemUndeletedIntegrationEvent(Guid Id) : INotification;
