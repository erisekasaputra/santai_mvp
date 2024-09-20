using MediatR;

namespace Core.Events.Catalog;

public record BrandDeletedIntegrationEvent(Guid Id) : INotification;
