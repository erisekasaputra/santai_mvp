using Account.Domain.Aggregates.ReferredAggregate;

namespace Account.API.Applications.Dtos.ResponseDtos;

public record ReferredProgramResponseDto(
    Guid ReferrerId,
    Guid ReferredUserId,
    string ReferralCode,
    DateTime ReferredDateUtc,
    ReferralStatus Status);
