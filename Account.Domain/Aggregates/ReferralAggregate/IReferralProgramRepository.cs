namespace Account.Domain.Aggregates.ReferralAggregate;

public interface IReferralProgramRepository
{
    Task<ReferralProgram?> GetByCodeAsync(string code);
}
