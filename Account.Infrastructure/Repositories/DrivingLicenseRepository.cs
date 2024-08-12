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

    public async Task<bool> GetAnyAcceptedByUserIdAsync(Guid id)
    {
        return await _context.DrivingLicenses
            .AsNoTracking().AnyAsync(x => x.UserId == id && x.VerificationStatus == VerificationState.Accepted);
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
}
