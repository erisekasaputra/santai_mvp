using Account.API.Applications.Dtos.ResponseDtos;
using Account.Domain.Aggregates.LoyaltyAggregate;

namespace Account.API.Mapper;

public static class LoyaltyProgramMapperExtension
{
    public static LoyaltyProgramResponseDto ToLoyaltyProgramResponseDto(this LoyaltyProgram program)
    {
        return new LoyaltyProgramResponseDto(program.LoyaltyUserId, program.LoyaltyPoints, program.LoyaltyTier);
    }
}
