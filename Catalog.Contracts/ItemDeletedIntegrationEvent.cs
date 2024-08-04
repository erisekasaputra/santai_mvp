using MediatR;

namespace Catalog.Contracts;

public record ItemDeletedIntegrationEvent(string Id) : INotification;
