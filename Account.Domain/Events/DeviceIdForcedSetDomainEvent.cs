using MediatR;

namespace Account.Domain.Events;

public record DeviceIdForcedSetDomainEvent(Guid Id, string DeviceId) : INotification;
