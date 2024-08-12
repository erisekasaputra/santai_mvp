namespace Account.Domain.Aggregates.DrivingLicenseAggregate;

public interface IDrivingLicenseRepository
{
    Task<bool> GetAnyAcceptedByUserIdAsync(Guid id);

    Task<bool> GetAnyByLicenseNumberAsync(string hashedLicenseNumber);

    Task<bool> GetAnyByLicenseNumberExcludingUserIdAsync(Guid userId, string hashedLicenseNumber);
}
