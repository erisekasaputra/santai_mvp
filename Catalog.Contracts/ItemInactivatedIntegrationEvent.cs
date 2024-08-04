using MediatR;

namespace Catalog.Contracts;

public record ItemInactivatedIntegrationEvent(string Id) : INotification;
