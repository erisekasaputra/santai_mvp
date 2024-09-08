using MediatR;

namespace Core.Events;

public record BrandUpdatedIntegrationEvent(Guid Id, string Name, string ImageUrl) : INotification;
