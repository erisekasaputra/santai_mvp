using MediatR;

namespace Core.Events;

public record BusinessLicenseAcceptedIntegrationEvent(Guid BusinessLicenseId) : INotification;
