using MediatR;

namespace Core.Events;

public record BusinessLicenseRejectedIntegrationEvent(Guid BusinessLicenseId) : INotification;
