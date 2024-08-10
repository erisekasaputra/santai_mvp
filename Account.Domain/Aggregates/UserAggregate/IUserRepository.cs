using Account.Domain.Enumerations;

namespace Account.Domain.Aggregates.UserAggregate;

public interface IUserRepository
{ 
    Task<User?> GetByIdAsync(Guid id);
    Task<User> CreateAsync(User user);
    void Update(User user);
    void Delete(User user);
    Task<bool> GetAnyByIdAsync(Guid id);
    Task<BusinessUser?> GetBusinessUserByIdAsync(Guid id);
    Task<RegularUser?> GetRegularUserByIdAsync(Guid id);
    Task<MechanicUser?> GetMechanicUserByIdAsync(Guid id); 
    Task<User?> GetByIdentitiesAsNoTrackingAsync(params (IdentityParameter, string)[] identity);
    Task<User?> GetByIdentitiesExcludingIdAsNoTrackingAsync(Guid id, params (IdentityParameter, string)[] identity);
    Task<bool> GetAnyByIdentitiesAsNoTrackingAsync(params (IdentityParameter, string)[] identity);
    Task<bool> GetAnyByIdentitiesExcludingIdAsNoTrackingAsync(Guid id, params (IdentityParameter, string)[] identity);
}
