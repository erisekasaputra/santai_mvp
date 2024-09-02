using MediatR;

namespace Account.Contracts;

public record BusinessLicenseAcceptedIntegrationEvent(Guid BusinessLicenseId) : INotification;
