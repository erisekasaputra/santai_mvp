using Account.Domain.Enumerations; 

namespace Account.Domain.Aggregates.UserAggregate;

public interface IStaffRepository
{
    Task<(int TotalCount, int TotalPages, IEnumerable<Staff> Staffs)> GetPaginatedStaffByUserIdAsync(Guid userId, int pageNumber, int pageSize);
    Task<IEnumerable<Staff>?> GetByIdentitiesAsNoTrackingAsync(params (IdentityParameter, IEnumerable<string>)[] parameters);
    Task<IEnumerable<Staff>?> GetByIdentitiesExcludingIdsAsNoTrackingAsync(params (IdentityParameter, IEnumerable<(Guid id, string identity)>)[] parameters);
    Task<bool> GetAnyByIdentitiesAsNoTrackingAsync(params (IdentityParameter, IEnumerable<string>)[] parameters);
    Task<bool> GetAnyByIdentitiesExcludingIdsAsNoTrackingAsync(params (IdentityParameter, IEnumerable<(Guid id, string identity)>)[] parameters);
    Task<Staff?> GetByBusinessUserIdAndStaffIdAsync(Guid userId, Guid staffId);
    Task<Staff> CreateAsync(Staff staff);
    Task<bool> GetAnyAsync(Guid id);
    Task<Staff?> GetByIdAsync(Guid id);
    Task<string?> GetTimeZoneByUserIdAndIdAsync(Guid userId, Guid staffId);
    Task<string?> GetEmailByUserIdAndIdAsync(Guid userId, Guid staffId);
    Task<string?> GetPhoneNumberByUserIdAndIdAsync(Guid userId, Guid staffId); 
    Task<string?> GetDeviceIdByUserAndByIdAsync(Guid userId, Guid staffId);
    void Update(Staff staff);
    void Delete(Staff staff); 
}
