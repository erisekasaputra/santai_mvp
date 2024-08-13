using Account.Domain.Aggregates.DrivingLicenseAggregate;
using Account.Domain.Enumerations;
using Microsoft.EntityFrameworkCore;

namespace Account.Infrastructure.Repositories;

public class DrivingLicenseRepository : IDrivingLicenseRepository
{
    private readonly AccountDbContext _context;

    public DrivingLicenseRepository(AccountDbContext context)
    {
        _context = context;
    }

    public async Task<DrivingLicense> CreateAsync(DrivingLicense drivingLicense)
    {
        var entries = await _context.DrivingLicenses.AddAsync(drivingLicense);
        return entries.Entity;
    }

    public async Task<DrivingLicense?> GetAcceptedByUserIdAsync(Guid id)
    {
        return await _context.DrivingLicenses
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.UserId == id && x.VerificationStatus == VerificationState.Accepted);
    }

    public async Task<bool> GetAnyByLicenseNumberAsync(string hashedLicenseNumber)
    {
        return await _context.DrivingLicenses
            .AsNoTracking().AnyAsync(x => x.HashedLicenseNumber == hashedLicenseNumber);
    }

    public async Task<bool> GetAnyByLicenseNumberExcludingUserIdAsync(Guid userId, string hashedLicenseNumber)
    {
        return await _context.DrivingLicenses
            .AsNoTracking().AnyAsync(x => x.HashedLicenseNumber == hashedLicenseNumber && x.UserId != userId);
    }

    public async Task<DrivingLicense?> GetByUserIdAndIdAsync(Guid userId, Guid licenseId)
    {
        return await _context.DrivingLicenses
            .FirstOrDefaultAsync(x => x.UserId == userId && x.Id == licenseId);
    }

    public async Task<DrivingLicense?> GetOrderWithAcceptedByUserIdAsync(Guid id)
    {
        return await _context.DrivingLicenses
            .Where(x => x.UserId == id)
            .OrderByDescending(x => x.VerificationStatus == VerificationState.Accepted)
            .ThenByDescending(x => x.VerificationStatus == VerificationState.Waiting)
            .FirstOrDefaultAsync();
    }

    public void Update(DrivingLicense license)
    {
        _context.DrivingLicenses.Update(license);
    }
}
