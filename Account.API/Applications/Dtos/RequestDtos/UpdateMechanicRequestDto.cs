using Account.API.Extensions;
using Core.Extensions;

namespace Account.API.Applications.Dtos.RequestDtos;
  
public class UpdateMechanicRequestDto (PersonalInfoRequestDto personalInfo, AddressRequestDto address, string timeZoneId)
{
    public required PersonalInfoRequestDto PersonalInfo { get; set; } = personalInfo;
    public required AddressRequestDto Address { get; set; } = address;
    public required string TimeZoneId { get; set; } = timeZoneId.Clean();
}

