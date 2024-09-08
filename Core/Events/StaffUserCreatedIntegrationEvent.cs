using MediatR;

namespace Core.Events;

public record StaffUserCreatedIntegrationEvent(StaffIntegrationEvent Staff) : INotification;