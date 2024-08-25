using MediatR;

namespace Account.Domain.Events;

public record PhoneNumberUpdatedDomainEvent(
    Guid Id,
    string? HashedPhoneNumber,
    string? NewHashedPhoneNumber,
    string? EncryptedPhoneNumber,
    string? NewEncryptedPhoneNumber) : INotification;
