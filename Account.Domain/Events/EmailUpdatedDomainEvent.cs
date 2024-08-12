using MediatR;

namespace Account.Domain.Events;

public record EmailUpdatedDomainEvent(
    Guid Id,
    string OldEmail,
    string NewEmail,
    string OldEncryptedEmail,
    string NewEncryptedEmail) : INotification;