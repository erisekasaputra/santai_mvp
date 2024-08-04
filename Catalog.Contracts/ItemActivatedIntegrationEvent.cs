using MediatR;

namespace Catalog.Contracts;

public record ItemActivatedIntegrationEvent(string Id) : INotification;
