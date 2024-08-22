using Account.Domain.Enumerations;
using Identity.Contracts.Enumerations;

namespace Account.Domain.Aggregates.UserAggregate;

public interface IUserRepository
{
    Task<BaseUser?> GetByIdAsync(Guid id);
    Task<BaseUser> CreateAsync(BaseUser user);
    void Update(BaseUser user);
    void Delete(BaseUser user);
    Task<bool> GetAnyByIdAsync(Guid id);
    Task<BusinessUser?> GetBusinessUserByIdAsync(Guid id);
    Task<RegularUser?> GetRegularUserByIdAsync(Guid id);
    Task<MechanicUser?> GetMechanicUserByIdAsync(Guid id); 
    Task<BaseUser?> GetByIdentitiesAsNoTrackingAsync(params (IdentityParameter, string?)[] identity);
    Task<BaseUser?> GetByIdentitiesExcludingIdAsNoTrackingAsync(Guid id, params (IdentityParameter, string?)[] identity);
    Task<bool> GetAnyByIdentitiesAsNoTrackingAsync(params (IdentityParameter, string?)[] identity);
    Task<bool> GetAnyByIdentitiesExcludingIdAsNoTrackingAsync(Guid id, params (IdentityParameter, string?)[] identity);
    Task<(int TotalCount, int TotalPages, IEnumerable<RegularUser> Brands)> GetPaginatedRegularUser(int pageNumber, int pageSize);
    Task<(int TotalCount, int TotalPages, IEnumerable<BusinessUser> Brands)> GetPaginatedBusinessUser(int pageNumber, int pageSize);
    Task<(int TotalCount, int TotalPages, IEnumerable<MechanicUser> Brands)> GetPaginatedMechanicUser(int pageNumber, int pageSize);
    Task<string?> GetTimeZoneById(Guid id);
    Task<string?> GetEmailById(Guid id);
    Task<string?> GetPhoneNumberById(Guid id);    
    Task<string?> GetDeviceIdByMechanicUserId(Guid id);    
    Task<string?> GetDeviceIdByRegularUserId(Guid id);    
    Task<UserType?> GetUserTypeById(Guid id);
}
