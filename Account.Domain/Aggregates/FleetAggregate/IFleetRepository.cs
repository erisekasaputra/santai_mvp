using Account.Domain.Enumerations;

namespace Account.Domain.Aggregates.FleetAggregate;

public interface IFleetRepository
{
    Task<(int TotalCount, int TotalPages, IEnumerable<Fleet> Fleets)>GetPaginatedFleetByUserId(Guid userId, int pageNumber, int pageSize);
    void Update(Fleet fleet);
    void Delete(Fleet fleet);
    Task<Fleet?> GetByUserIdAndIdAsync(Guid userId, Guid fleetId);
    Task<Fleet> CreateAsync(Fleet fleet);   
    Task<bool> GetAnyByIdentityAsync(params (FleetLegalParameter parameter, string hashedValue)[] clauses);
    Task<Fleet?> GetByIdentityAsync(params (FleetLegalParameter parameter, string hashedValue)[] clauses);
    Task<Fleet?> GetByIdentityExcludingUserIdAsync(Guid userId, params (FleetLegalParameter parameter, string hashedValue)[] clauses);
    Task<IEnumerable<Fleet>> GetByIdsAsync(IEnumerable<Guid> ids);
    Task DeleteByUserId(Guid id);
    Task<IEnumerable<Fleet>> GetByStaffIdAsync(Guid staffId);
    void UpdateRange(IEnumerable<Fleet> fleets);
}
