using MediatR;

namespace Catalog.Contracts;

public record BrandUpdatedIntegrationEvent(Guid Id, string Name, string ImageUrl) : INotification;
