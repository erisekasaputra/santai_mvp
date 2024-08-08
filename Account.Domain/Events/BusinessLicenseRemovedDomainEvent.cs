using Account.Domain.Aggregates.BusinessLicenseAggregate;
using MediatR;

namespace Account.Domain.Events;

public record BusinessLicenseRemovedDomainEvent(BusinessLicense BusinessLicense) : INotification;
