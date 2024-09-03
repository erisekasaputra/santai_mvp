using MediatR;

namespace Catalog.Contracts;

public record ItemInactivatedIntegrationEvent(Guid Id) : INotification;
