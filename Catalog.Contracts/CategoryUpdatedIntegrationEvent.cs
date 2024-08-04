using MediatR;

namespace Catalog.Contracts;

public record CategoryUpdatedIntegrationEvent(string Id, string Name, string ImageUrl) : INotification;