

namespace Account.Domain.Aggregates.BusinessLicenseAggregate;

public interface IBusinessLicenseRepository
{
    Task<(int TotalCount, int TotalPages, IEnumerable<BusinessLicense> BusinessLicenses)> GetPaginatedBusinessLicenseByUserId(Guid userId, int pageNumber, int pageSize);
    Task<BusinessLicense?> GetByBusinessUserIdAndBusinessLicenseIdAsync(Guid businessUserId, Guid businessLicenseId);
    Task<IEnumerable<BusinessLicense>?> GetByLicenseNumberAsNoTrackingAsync(IEnumerable<string> licenseNumbers);
    Task<IEnumerable<BusinessLicense>?> GetAcceptedStatusByLicenseNumbersAsNoTrackingAsync(IEnumerable<string> licenseNumbers);
    Task<BusinessLicense?> GetAcceptedStatusByLicenseNumberAsNoTrackingAsync(string licenseNumber); 
    Task<BusinessLicense?> GetByIdAsync(Guid id); 
    Task<BusinessLicense> CreateAsync(BusinessLicense license);
    Task<bool> GetAnyByIdAsync(Guid id);
    void Update(BusinessLicense businessLicense);  
    void Delete(BusinessLicense businessLicense);
}
