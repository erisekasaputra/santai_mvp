using MediatR;

namespace Account.Domain.Events;

public record EmailVerifiedDomainEvent(Guid Id, string HashedEmail, string EncryptedEmail) : INotification;
