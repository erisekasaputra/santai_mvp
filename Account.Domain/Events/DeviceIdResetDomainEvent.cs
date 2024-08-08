using MediatR;

namespace Account.Domain.Events;

public record DeviceIdResetDomainEvent(Guid Id) : INotification;
