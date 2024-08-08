using Account.Domain.Aggregates.ReferralAggregate;
using Microsoft.EntityFrameworkCore;
namespace Account.Infrastructure.Repositories;

public class ReferralProgramRepository : IReferralProgramRepository
{
    private readonly AccountDbContext _context;

    public ReferralProgramRepository(AccountDbContext context)
    {
        _context = context;
    }

    public async Task<ReferralProgram?> GetByCodeAsync(string code)
    {
        var referralProgram = await _context.ReferralPrograms.FirstOrDefaultAsync(x => 
            x.ReferralCode == code);

        return referralProgram;
    }
}
