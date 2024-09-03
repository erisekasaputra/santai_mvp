using MediatR;

namespace Catalog.Contracts;

public record BrandDeletedIntegrationEvent(Guid Id) : INotification;
