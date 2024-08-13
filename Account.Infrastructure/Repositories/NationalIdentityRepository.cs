 
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

    public async Task<NationalIdentity?> CreateAsync(NationalIdentity nationalIdentity)
    {
        var entry = await _context.NationalIdentities.AddAsync(nationalIdentity);
        return entry.Entity;
    }

    public async Task<NationalIdentity?> GetAcceptedByUserIdAsync(Guid id)
    {
        return await _context.NationalIdentities
            .AsNoTracking().FirstOrDefaultAsync(x => x.UserId == id && x.VerificationStatus == VerificationState.Accepted);
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

    public async Task<NationalIdentity?> GetByUserIdAndIdAsync(Guid userId, Guid identityId)
    {
        return await _context.NationalIdentities.FirstOrDefaultAsync(x => x.UserId == userId && x.Id == identityId);
    }

    public void Update(NationalIdentity identity)
    {
        _context.NationalIdentities.Update(identity);
    }

    public async Task<NationalIdentity?> GetOrderWithAcceptedByUserIdAsync(Guid id)
    {
        return await _context.NationalIdentities
            .Where(x => x.UserId == id)
            .OrderByDescending(x => x.VerificationStatus == VerificationState.Accepted)
            .ThenByDescending(x => x.VerificationStatus == VerificationState.Waiting)
            .FirstOrDefaultAsync();
    }
}
