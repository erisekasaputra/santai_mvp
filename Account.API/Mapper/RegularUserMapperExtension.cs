using Account.API.Applications.Dtos.ResponseDtos;
using Account.Domain.Aggregates.UserAggregate;

namespace Account.API.Mapper;

public static class RegularUserMapperExtension
{
    public static RegularUserResponseDto ToRegularUserResponseDto(this RegularUser user)
    {
        return new RegularUserResponseDto(user.Id, user.Username, user.Email, user.PhoneNumber, user.TimeZoneId, user.Address.ToAddressResponseDto(), user.PersonalInfo.ToPersonalInfoResponseDto(user.TimeZoneId));
    }
}
