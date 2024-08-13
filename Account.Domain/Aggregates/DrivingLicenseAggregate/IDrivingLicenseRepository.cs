namespace Account.Domain.Aggregates.DrivingLicenseAggregate;

public interface IDrivingLicenseRepository
{
    Task<DrivingLicense?> GetOrderWithAcceptedByUserIdAsync(Guid id);

    Task<DrivingLicense?> GetAcceptedByUserIdAsync(Guid id);

    Task<bool> GetAnyByLicenseNumberAsync(string hashedLicenseNumber);

    Task<bool> GetAnyByLicenseNumberExcludingUserIdAsync(Guid userId, string hashedLicenseNumber);

    Task<DrivingLicense?> GetByUserIdAndIdAsync(Guid userId, Guid licenseId);

    void Update(DrivingLicense license);

    Task<DrivingLicense> CreateAsync(DrivingLicense drivingLicense);

}
