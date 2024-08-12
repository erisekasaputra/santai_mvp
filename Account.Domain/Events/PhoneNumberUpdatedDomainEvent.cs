using MediatR;

namespace Account.Domain.Events;

public record PhoneNumberUpdatedDomainEvent(
    Guid Id,
    string OldPhoneNumber,
    string NewPhoneNumber,
    string OldEncryptedPhoneNumber,
    string NewEncryptedPhoneNumber) : INotification;
