using Account.Domain.Aggregates.NationalIdentityAggregate;

namespace Account.Infrastructure.Repositories;

public class NationalIdentityRepository : INationalIdentityRepository
{
    private readonly AccountDbContext _context;

    public NationalIdentityRepository(AccountDbContext context)
    {
        _context = context;
    }
}
