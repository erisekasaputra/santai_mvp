using Account.Domain.Enumerations;

namespace Account.API.Applications.Dtos.ResponseDtos;

public record ReferralProgramResponseDto(string? ReferralCode, int? RewardPoint);
