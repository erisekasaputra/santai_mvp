using MediatR;

namespace Core.Events.Catalog;

public record CategoryUpdatedIntegrationEvent(Guid Id, string Name, string ImageUrl) : INotification;