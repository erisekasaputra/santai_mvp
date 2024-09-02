using MediatR;

namespace Account.Contracts;

public record BusinessLicenseRejectedIntegrationEvent(Guid BusinessLicenseId) : INotification;
