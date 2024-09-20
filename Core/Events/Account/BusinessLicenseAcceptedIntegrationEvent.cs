using MediatR;

namespace Core.Events.Account;

public record BusinessLicenseAcceptedIntegrationEvent(Guid BusinessLicenseId) : INotification;
