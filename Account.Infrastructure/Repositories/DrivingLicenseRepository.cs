using Account.Domain.Aggregates.DrivingLicenseAggregate;

namespace Account.Infrastructure.Repositories;

public class DrivingLicenseRepository : IDrivingLicenseRepository
{
    private readonly AccountDbContext _context;

    public DrivingLicenseRepository(AccountDbContext context)
    {
        _context = context;
    }
}
