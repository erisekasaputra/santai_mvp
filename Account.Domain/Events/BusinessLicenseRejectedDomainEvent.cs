using MediatR;

namespace Account.Domain.Events;

public record BusinessLicenseRejectedDomainEvent(Guid BusinessLicenseId) : INotification;
