namespace Account.API.Applications.Dtos.RequestDtos;

public class UpdateRegularUserRequestDto(AddressRequestDto address, string timeZoneId, PersonalInfoRequestDto personalInfo)
{
    public required AddressRequestDto Address { get; set; } = address;
    public required string TimeZoneId { get; set; } = timeZoneId;
    public required PersonalInfoRequestDto PersonalInfo { get; set; } = personalInfo;
}
