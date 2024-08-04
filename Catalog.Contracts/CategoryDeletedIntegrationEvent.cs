using MediatR;

namespace Catalog.Contracts;

public record CategoryDeletedIntegrationEvent(string Id) : INotification;
