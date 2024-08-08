using Account.Domain.Aggregates.BusinessLicenseAggregate;
using Microsoft.EntityFrameworkCore;

namespace Account.Infrastructure.Repositories;

public class BusinessLicenseRepository : IBusinessLicenseRepository
{
    private readonly AccountDbContext _context;

    public BusinessLicenseRepository(AccountDbContext context)
    {
        _context = context; 
    }

    public async Task<BusinessLicense?> GetAcceptedByNumberAsNoTrackAsync(string licenseNumber)
    { 
        string normalizedLicense = licenseNumber.Trim().ToLower();
        return await _context.BusinessLicenses.AsNoTracking()
            .FirstOrDefaultAsync(x => x.LicenseNumber.Trim().ToLower() == normalizedLicense 
            && x.VerificationStatus == Domain.Enumerations.VerificationState.Accepted);
    }

    public async Task<IEnumerable<BusinessLicense>?> GetAcceptedByNumbersAsNoTrackAsync(IEnumerable<string> licenseNumbers)
    {
        var normalizedLicense = licenseNumbers.Select(x => x.Trim().ToLower());
        return await _context.BusinessLicenses
            .AsNoTracking()
            .Where(x => normalizedLicense.Contains(x.LicenseNumber.Trim().ToLower()) && x.VerificationStatus == Domain.Enumerations.VerificationState.Accepted)
            .ToListAsync();
    }


    public async Task<BusinessLicense?> GetByIdAsync(Guid id)
    {
        return await _context.BusinessLicenses.FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task<IEnumerable<BusinessLicense>?> GetByNumberAsNoTrackAsync(IEnumerable<string> licenseNumbers)
    {
        var normalizedLicenses = licenseNumbers.Select(b =>
                b.Trim().ToLower()).ToList();

        return await _context.BusinessLicenses.Where(x =>
            normalizedLicenses.Contains(x.LicenseNumber.Trim().ToLower())
            && x.VerificationStatus != Domain.Enumerations.VerificationState.Rejected)
                    .AsNoTracking().IgnoreQueryFilters().ToListAsync();
    }

    public void Update(BusinessLicense license)
    {
        _context.BusinessLicenses.Update(license);
    }
}
