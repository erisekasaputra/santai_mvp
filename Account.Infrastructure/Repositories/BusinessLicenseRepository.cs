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

    public async Task<BusinessLicense> CreateAsync(BusinessLicense license)
    {
        var entity = await _context.BusinessLicenses.AddAsync(license);

        return entity.Entity;
    }

    public void Delete(BusinessLicense license)
    {
        _context.BusinessLicenses.Remove(license);
    }
     

    public async Task<BusinessLicense?> GetAcceptedStatusByLicenseNumberAsNoTrackingAsync(string licenseNumber)
    {  
        return await _context.BusinessLicenses.AsNoTracking()
            .FirstOrDefaultAsync(x => x.HashedLicenseNumber == licenseNumber
                && x.VerificationStatus == Domain.Enumerations.VerificationState.Accepted);
    }

    public async Task<IEnumerable<BusinessLicense>?> GetAcceptedStatusByLicenseNumbersAsNoTrackingAsync(IEnumerable<string> licenseNumbers)
    { 
        return await _context.BusinessLicenses
            .AsNoTracking()
            .Where(x => licenseNumbers.Contains(x.HashedLicenseNumber) 
                && x.VerificationStatus == Domain.Enumerations.VerificationState.Accepted)
            .ToListAsync();
    }

    public async Task<bool> GetAnyByIdAsync(Guid id)
    {
        return await _context.BusinessLicenses.AnyAsync(x => x.Id == id);
    }

    public async Task<BusinessLicense?> GetByBusinessUserIdAndBusinessLicenseIdAsync(Guid businessUserId, Guid businessLicenseId)
    {
        return await _context.BusinessLicenses.FirstOrDefaultAsync(x => x.BusinessUserId == businessUserId && x.Id == businessLicenseId);
    }

    public async Task<BusinessLicense?> GetByIdAsync(Guid id)
    {
        return await _context.BusinessLicenses.FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task<IEnumerable<BusinessLicense>?> GetByLicenseNumberAsNoTrackingAsync(IEnumerable<string> licenseNumbers)
    { 
        return await _context.BusinessLicenses.Where(x =>
            licenseNumbers.Contains(x.HashedLicenseNumber)
            && x.VerificationStatus != Domain.Enumerations.VerificationState.Rejected)
                    .AsNoTracking().IgnoreQueryFilters().ToListAsync();
    }

    public void Update(BusinessLicense license)
    {
        _context.BusinessLicenses.Update(license);
    }
}
