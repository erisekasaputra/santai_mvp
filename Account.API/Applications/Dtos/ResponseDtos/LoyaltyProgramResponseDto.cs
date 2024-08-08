using Account.Domain.Enumerations;

namespace Account.API.Applications.Dtos.ResponseDtos;

public record LoyaltyProgramResponseDto(Guid UserId, int Points, LoyaltyTier Tier);
