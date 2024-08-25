namespace Account.API.Applications.Dtos.ResponseDtos;

public record MechanicUserResponseDto(
    Guid Id, 
    string? Email,
    string? PhoneNumber,
    string TimeZoneId,
    LoyaltyProgramResponseDto LoyaltyProgram,
    AddressResponseDto Address,
    IEnumerable<CertificationResponseDto>? Certifications,
    DrivingLicenseResponseDto? DrivingLicense,
    NationalIdentityResponseDto? NationalIdentity);