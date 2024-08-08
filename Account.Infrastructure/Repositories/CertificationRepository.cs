using Account.Domain.Aggregates.CertificationAggregate;

namespace Account.Infrastructure.Repositories;

public class CertificationRepository : ICertificationRepository
{
    private readonly AccountDbContext _context;

    public CertificationRepository(AccountDbContext context)
    {
        _context = context;
    }
}
