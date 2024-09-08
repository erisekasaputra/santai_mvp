using MediatR;

namespace Core.Events;

public record CategoryUpdatedIntegrationEvent(Guid Id, string Name, string ImageUrl) : INotification;