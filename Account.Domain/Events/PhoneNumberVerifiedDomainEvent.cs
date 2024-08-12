using MediatR;

namespace Account.Domain.Events;

public record PhoneNumberVerifiedDomainEvent(Guid Id, string HashedPhoneNumber, string EncryptedPhoneNumber) : INotification;
