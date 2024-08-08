namespace Account.API.Applications.Dtos.ResponseDtos;

public record RegularUserResponseDto(
    Guid IdentityId,
    string Username,
    string Email,
    string PhoneNumber,
    string TimeZoneId,
    AddressResponseDto Address,
    PersonalInfoResponseDto PersonalInfo,
    string DeviceId);