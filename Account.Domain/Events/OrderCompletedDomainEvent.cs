using MediatR;

namespace Account.Domain.Events;

public record OrderCompletedDomainEvent(Guid OrderId) : INotification;
