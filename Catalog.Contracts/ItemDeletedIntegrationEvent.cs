using MediatR;

namespace Catalog.Contracts;

public record ItemDeletedIntegrationEvent(Guid Id) : INotification;
