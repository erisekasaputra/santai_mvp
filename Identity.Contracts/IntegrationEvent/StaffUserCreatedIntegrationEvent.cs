using Identity.Contracts.EventEntity;
using MediatR;

namespace Identity.Contracts.IntegrationEvent;

public record StaffUserCreatedIntegrationEvent(StaffEvent Staff) : INotification;