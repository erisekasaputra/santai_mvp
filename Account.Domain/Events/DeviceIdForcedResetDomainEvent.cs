using MediatR;

namespace Account.Domain.Events;

public record DeviceIdForcedResetDomainEvent(Guid Id) : INotification;
