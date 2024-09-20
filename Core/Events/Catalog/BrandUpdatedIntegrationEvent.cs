using MediatR;

namespace Core.Events.Catalog;

public record BrandUpdatedIntegrationEvent(Guid Id, string Name, string ImageUrl) : INotification;
