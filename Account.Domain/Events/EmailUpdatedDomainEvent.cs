using MediatR;

namespace Account.Domain.Events;

public record EmailUpdatedDomainEvent(
    Guid Id,
    string? HashedEmail,
    string NewHashedEmail,
    string? EncryptedEmail,
    string NewEncryptedEmail) : INotification;