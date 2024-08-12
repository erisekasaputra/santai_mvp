namespace Account.Domain.Aggregates.NationalIdentityAggregate;

public interface INationalIdentityRepository
{
    Task<bool> GetAnyAcceptedByUserIdAsync(Guid id);

    Task<bool> GetAnyByIdentityNumberAsync(string hashedIdentityNumber);

    Task<bool> GetAnyByIdentityNumberExcludingUserIdAsync(Guid userId, string hashedIdentityNumber);
}
