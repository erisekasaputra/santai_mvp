using Account.Domain.Aggregates.BusinessLicenseAggregate;
using MediatR;

namespace Account.Domain.Events;

public record BusinessLicenseAddedDomainEvent(BusinessLicense BusinessLicense) : INotification;
