using Account.API.Applications.Dtos.ResponseDtos;
using Account.Domain.Aggregates.UserAggregate;

namespace Account.API.Mapper;

public static class BusinessUserMapperExtension
{
    public static BusinessUserResponseDto? ToBusinessUserResponseDto(this User? user)
    { 
        if (user is not null && user is BusinessUser businessUser)
        {
            return new BusinessUserResponseDto(
                businessUser.Id,
                businessUser.Username,
                businessUser.Email,
                businessUser.PhoneNumber,
                businessUser.TimeZoneId,
                businessUser.Address.ToAddressResponseDto(),
                businessUser.BusinessName,
                businessUser.BusinessName,
                businessUser.TaxId,
                businessUser.WebsiteUrl,
                businessUser.Description,
                businessUser.LoyaltyProgram?.ToLoyaltyProgramResponseDto(),
                businessUser.BusinessLicenses?.ToBusinessLicenseResponseDtos(),
                businessUser.Staffs?.ToStaffResponseDtos());
        } 

        return default;
    }
}
