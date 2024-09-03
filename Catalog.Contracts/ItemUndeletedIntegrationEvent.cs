using MediatR;

namespace Catalog.Contracts;

public record ItemUndeletedIntegrationEvent(Guid Id) : INotification;
