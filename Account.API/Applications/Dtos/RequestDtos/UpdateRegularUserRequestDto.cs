namespace Account.API.Applications.Dtos.RequestDtos;

public class UpdateRegularUserRequestDto(AddressRequestDto address, string timeZoneId, PersonalInfoRequestDto personalInfo)
{
    public AddressRequestDto Address { get; } = address;
    public string TimeZoneId { get; } = timeZoneId;
    public PersonalInfoRequestDto PersonalInfo { get; } = personalInfo;
}
