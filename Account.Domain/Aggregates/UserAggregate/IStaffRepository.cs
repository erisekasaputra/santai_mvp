using Account.Domain.Enumerations; 

namespace Account.Domain.Aggregates.UserAggregate;

public interface IStaffRepository
{
    Task<IEnumerable<Staff>?> GetByIdentitiesAsNoTrackAsync(params (IdentityParameter, IEnumerable<string>)[] parameters);
    Task<IEnumerable<Staff>?> GetByIdentitiesExcludingIdsAsNoTrackAsync(params (IdentityParameter, IEnumerable<(Guid id, string identity)>)[] parameters);
}
