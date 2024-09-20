using MediatR;

namespace Core.Events.Account;

public record BusinessLicenseRejectedIntegrationEvent(Guid BusinessLicenseId) : INotification;
