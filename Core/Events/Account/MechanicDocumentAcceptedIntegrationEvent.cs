using MediatR;

namespace Core.Events.Account;

public record MechanicDocumentAcceptedIntegrationEvent(Guid MechanicId, string Name) : INotification;
