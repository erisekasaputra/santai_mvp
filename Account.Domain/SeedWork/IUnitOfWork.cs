using Account.Domain.Aggregates.BusinessLicenseAggregate;
using Account.Domain.Aggregates.CertificationAggregate;
using Account.Domain.Aggregates.DrivingLicenseAggregate;
using Account.Domain.Aggregates.LoyaltyAggregate;
using Account.Domain.Aggregates.NationalIdentityAggregate;
using Account.Domain.Aggregates.ReferralAggregate;
using Account.Domain.Aggregates.ReferredAggregate;
using Account.Domain.Aggregates.UserAggregate;
using System.Data;

namespace Account.Domain.SeedWork;

public interface IUnitOfWork
{
    IUserRepository Users { get; }

    IBusinessLicenseRepository BusinessLicenses { get; }

    ICertificationRepository Certifications { get; }

    IDrivingLicenseRepository DrivingLicenses { get; }

    INationalIdentityRepository NationalIdentities { get; }

    ILoyaltyProgramRepository LoyaltyPrograms { get; }

    IReferralProgramRepository ReferralPrograms { get; }

    IReferredProgramRepository ReferredPrograms { get; }

    IStaffRepository Staffs { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);

    Task BeginTransactionAsync(IsolationLevel isolationLevel = IsolationLevel.ReadCommitted, CancellationToken cancellationToken = default);

    Task CommitTransactionAsync(CancellationToken cancellationToken = default);

    Task RollbackTransactionAsync(CancellationToken cancellationToken = default);

    Task DispatchDomainEventsAsync(CancellationToken token = default);
}
