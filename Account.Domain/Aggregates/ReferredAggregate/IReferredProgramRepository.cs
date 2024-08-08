namespace Account.Domain.Aggregates.ReferredAggregate;

public interface IReferredProgramRepository
{
    Task<ReferredProgram?> GetReferredProgramByReferrerAndReferredUserAsync(Guid referrer, Guid referred);

    Task<ReferredProgram> CreateReferredProgramAsync(ReferredProgram referredProgram);
}
