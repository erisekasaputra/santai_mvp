using Account.API.Applications.Dtos.RequestDtos;
using Account.API.Applications.Dtos.ResponseDtos;
using Account.API.Extensions;
using Account.Domain.ValueObjects;

namespace Account.API.Mapper;

public static class PersonalInfoMapper
{
    public static PersonalInfo ToPersonalInfo(this PersonalInfoRequestDto personalInfo, string timeZoneId)
    {
        return new PersonalInfo(personalInfo.FirstName, personalInfo.MiddleName, personalInfo.LastName, personalInfo.DateOfBirth.FromLocalToUtc(timeZoneId), personalInfo.Gender, personalInfo.ProfilePictureUrl);
    }

    public static PersonalInfoResponseDto ToPersonalInfoResponseDto(this PersonalInfo personalInfo, string timeZoneId)
    {
        return new PersonalInfoResponseDto(personalInfo.FirstName, personalInfo.MiddleName, personalInfo.LastName, personalInfo.DateOfBirthUtc.FromUtcToLocal(timeZoneId), personalInfo.Gender, personalInfo.ProfilePictureUrl);
    }
}
