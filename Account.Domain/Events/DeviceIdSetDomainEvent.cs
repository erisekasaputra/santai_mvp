using MediatR;

namespace Account.Domain.Events;

public record DeviceIdSetDomainEvent(Guid Id, string DeviceId) : INotification;
