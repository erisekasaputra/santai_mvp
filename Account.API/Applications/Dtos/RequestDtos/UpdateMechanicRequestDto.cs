using Account.API.Extensions;
using Core.Extensions;

namespace Account.API.Applications.Dtos.RequestDtos;
  
public class UpdateMechanicRequestDto (PersonalInfoRequestDto personalInfo, AddressRequestDto address, string timeZoneId)
{
    public PersonalInfoRequestDto PersonalInfo = personalInfo;
    public AddressRequestDto Address { get; } = address;
    public string TimeZoneId { get; } = timeZoneId.Clean();
}

