using MediatR;

namespace Catalog.Contracts;

public record BrandDeletedIntegrationEvent(string Id) : INotification;
