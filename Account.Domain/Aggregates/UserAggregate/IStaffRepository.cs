using Account.Domain.Enumerations; 

namespace Account.Domain.Aggregates.UserAggregate;

public interface IStaffRepository
{
    Task<IEnumerable<Staff>?> GetByIdsAsNoTrackAsync(params (IdentityParameter, IEnumerable<string>)[] parameters);
    Task<IEnumerable<Staff>?> GetExcludingIdsAsNoTrackAsync(params (IdentityParameter, IEnumerable<(Guid id, string identity)>)[] parameters);
}
