using Account.Domain.Aggregates.LoyaltyAggregate;

namespace Account.Infrastructure.Repositories;

public class LoyaltyProgramRepository : ILoyaltyProgramRepository
{
    private readonly AccountDbContext _context;

    public LoyaltyProgramRepository(AccountDbContext context)
    {
        _context = context;
    }
}
