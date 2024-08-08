

namespace Account.Domain.Aggregates.BusinessLicenseAggregate;

public interface IBusinessLicenseRepository
{
    Task<IEnumerable<BusinessLicense>?> GetByNumberAsNoTrackAsync(IEnumerable<string> licenseNumbers);
    Task<IEnumerable<BusinessLicense>?> GetAcceptedByNumbersAsNoTrackAsync(IEnumerable<string> licenseNumbers);
    Task<BusinessLicense?> GetAcceptedByNumberAsNoTrackAsync(string licenseNumber); 
    Task<BusinessLicense?> GetByIdAsync(Guid id); 
    void Update(BusinessLicense license); 
}
