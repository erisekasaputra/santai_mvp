using Account.Domain.Enumerations;

namespace Account.Domain.Aggregates.UserAggregate;

public interface IUserRepository
{ 
    Task<User?> GetUserByIdAsync(Guid id);
    Task<User> CreateUserAsync(User user);
    void UpdateUser(User user);
    void DeleteUser(User user);
    Task<BusinessUser?> GetBusinessUserByIdAsync(Guid id);
    Task<RegularUser?> GetRegularUserByIdAsync(Guid id);
    Task<MechanicUser?> GetMechanicUserByIdAsync(Guid id); 
    Task<User?> GetByIdentityAsNoTrackAsync(params (IdentityParameter, string)[] identity);
    Task<User?> GetExcludingIdentityAsNoTrackAsync(Guid id, params (IdentityParameter, string)[] identity); 
}
