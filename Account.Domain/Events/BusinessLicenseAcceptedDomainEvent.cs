using MediatR;

namespace Account.Domain.Events;

public record BusinessLicenseAcceptedDomainEvent(Guid BusinessLicenseId) : INotification; 
