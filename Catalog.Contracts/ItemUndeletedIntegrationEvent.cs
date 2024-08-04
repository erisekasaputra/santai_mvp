using MediatR;

namespace Catalog.Contracts;

public record ItemUndeletedIntegrationEvent(string Id) : INotification;
