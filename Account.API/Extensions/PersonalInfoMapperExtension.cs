using Account.API.Applications.Dtos.RequestDtos;
using Account.API.Applications.Dtos.ResponseDtos;
using Account.Domain.ValueObjects;
using Core.Extensions;

namespace Account.API.Extensions;

public static class PersonalInfoMapperExtension
{
    public static PersonalInfo ToPersonalInfo(this PersonalInfoRequestDto personalInfo, string timeZoneId)
    { 

        return new PersonalInfo(personalInfo.FirstName, personalInfo.MiddleName, personalInfo.LastName, personalInfo.DateOfBirth?.FromLocalToUtc(timeZoneId) ?? DateTime.UtcNow, personalInfo.Gender, personalInfo.ProfilePictureUrl);
    }

    public static PersonalInfoResponseDto ToPersonalInfoResponseDto(this PersonalInfo personalInfo, string timeZoneId)
    {
        return new PersonalInfoResponseDto(personalInfo.FirstName, personalInfo.MiddleName, personalInfo.LastName, personalInfo.DateOfBirthUtc.FromUtcToLocal(timeZoneId), personalInfo.Gender, personalInfo.ProfilePictureUrl);
    }
}
