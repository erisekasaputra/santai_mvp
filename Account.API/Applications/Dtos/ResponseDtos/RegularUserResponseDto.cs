namespace Account.API.Applications.Dtos.ResponseDtos;

public record RegularUserResponseDto( 
    Guid Id, 
    string? Email,
    string? PhoneNumber,
    string TimeZoneId,
    AddressResponseDto Address,
    LoyaltyProgramResponseDto? LoyaltyProgram,
    ReferralProgramResponseDto? Referral,
    PersonalInfoResponseDto PersonalInfo,
    IEnumerable<FleetResponseDto> Fleets);