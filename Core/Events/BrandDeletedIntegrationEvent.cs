using MediatR;

namespace Core.Events;

public record BrandDeletedIntegrationEvent(Guid Id) : INotification;
