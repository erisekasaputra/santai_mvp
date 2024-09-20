using MediatR;

namespace Core.Events.Account;

public record StaffUserCreatedIntegrationEvent(StaffIntegrationEvent Staff) : INotification;