namespace Account.API.Applications.Dtos.ResponseDtos;

public record RegularUserResponseDto( 
    Guid Id,
    string Username,
    string Email,
    string PhoneNumber,
    string TimeZoneId,
    AddressResponseDto Address,
    PersonalInfoResponseDto PersonalInfo);