namespace Account.Domain.Aggregates.NationalIdentityAggregate;

public interface INationalIdentityRepository
{
    Task<NationalIdentity?> GetOrderWithAcceptedByUserIdAsync(Guid id);

    Task<NationalIdentity?> GetAcceptedByUserIdAsync(Guid id);

    Task<bool> GetAnyByIdentityNumberAsync(string hashedIdentityNumber);

    Task<bool> GetAnyByIdentityNumberExcludingUserIdAsync(Guid userId, string hashedIdentityNumber);

    Task<NationalIdentity?> GetByUserIdAndIdAsync(Guid userId, Guid identityId);

    void Update(NationalIdentity identity);

    Task<NationalIdentity?> CreateAsync(NationalIdentity nationalIdentity);
}
