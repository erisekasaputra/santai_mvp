using Account.Domain.Aggregates.NationalIdentityAggregate;
using Account.Domain.Enumerations;
using Microsoft.EntityFrameworkCore;

namespace Account.Infrastructure.Repositories;

public class NationalIdentityRepository : INationalIdentityRepository
{
    private readonly AccountDbContext _context;

    public NationalIdentityRepository(AccountDbContext context)
    {
        _context = context;
    }

    public async Task<bool> GetAnyAcceptedByUserIdAsync(Guid id)
    {
        return await _context.DrivingLicenses
            .AsNoTracking().AnyAsync(x => x.UserId == id && x.VerificationStatus == VerificationState.Accepted);
    }

    public async Task<bool> GetAnyByIdentityNumberAsync(string hashedIdentityNumber)
    {
        return await _context.NationalIdentities
            .AsNoTracking().AnyAsync(x => x.HashedIdentityNumber == hashedIdentityNumber);
    }

    public async Task<bool> GetAnyByIdentityNumberExcludingUserIdAsync(Guid userId, string hashedIdentityNumber)
    {
        return await _context.NationalIdentities
            .AsNoTracking().AnyAsync(x => x.HashedIdentityNumber == hashedIdentityNumber && x.UserId != userId);
    }
}
