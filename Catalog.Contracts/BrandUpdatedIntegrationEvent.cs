using MediatR;

namespace Catalog.Contracts;

public record BrandUpdatedIntegrationEvent(string Id, string Name, string ImageUrl) : INotification;
