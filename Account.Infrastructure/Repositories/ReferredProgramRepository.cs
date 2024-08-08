using Account.Domain.Aggregates.ReferredAggregate;
using Microsoft.EntityFrameworkCore;

namespace Account.Infrastructure.Repositories;

public class ReferredProgramRepository : IReferredProgramRepository
{
    private readonly AccountDbContext _context;

    public ReferredProgramRepository(AccountDbContext context)
    {
        _context = context;
    }

    public async Task<ReferredProgram> CreateReferredProgramAsync(ReferredProgram referredProgram)
    {
        var entry = await _context.ReferredPrograms.AddAsync(referredProgram); 
        return entry.Entity;
    }

    public async Task<ReferredProgram?> GetReferredProgramByReferrerAndReferredUserAsync(Guid referrer, Guid referred)
    {
        return await _context.ReferredPrograms.FirstOrDefaultAsync(x => x.ReferrerId == referrer && x.ReferredUserId == referred);
    }
}
