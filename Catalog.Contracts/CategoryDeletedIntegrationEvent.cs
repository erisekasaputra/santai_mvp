using MediatR;

namespace Catalog.Contracts;

public record CategoryDeletedIntegrationEvent(Guid Id) : INotification;
