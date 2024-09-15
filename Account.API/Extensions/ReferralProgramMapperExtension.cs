using Account.API.Applications.Dtos.ResponseDtos;
using Account.Domain.Aggregates.ReferralAggregate;

namespace Account.API.Extensions;

public static class ReferralProgramMapperExtension
{
    public static ReferralProgramResponseDto? ToReferralProgramResponseDto(this ReferralProgram? referralProgram)
    {
        if (referralProgram is null)
        {
            return null;
        }

        if (referralProgram.ValidDateUtc <= DateTime.UtcNow)
        {
            return null;
        }

        return new ReferralProgramResponseDto(referralProgram.ReferralCode, referralProgram.RewardPoint);
    }
}
