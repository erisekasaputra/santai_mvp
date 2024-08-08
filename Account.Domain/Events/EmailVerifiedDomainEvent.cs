using MediatR;

namespace Account.Domain.Events;

public record EmailVerifiedDomainEvent(Guid Id, string Email) : INotification;
