using MediatR;

namespace Core.Events.Account;

public record MechanicDocumentRejectedIntegrationEvent(Guid MechanicId, string Name) : INotification;
