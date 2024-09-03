using MediatR;

namespace Catalog.Contracts;

public record ItemActivatedIntegrationEvent(Guid Id) : INotification;
