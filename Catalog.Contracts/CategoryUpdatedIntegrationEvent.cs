using MediatR;

namespace Catalog.Contracts;

public record CategoryUpdatedIntegrationEvent(Guid Id, string Name, string ImageUrl) : INotification;