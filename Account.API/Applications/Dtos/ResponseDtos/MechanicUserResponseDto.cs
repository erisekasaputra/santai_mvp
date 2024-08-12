namespace Account.API.Applications.Dtos.ResponseDtos;

public record MechanicUserResponseDto( 
    string Username,
    string Email,
    string PhoneNumber,
    string TimeZoneId,
    AddressResponseDto Address,
    IEnumerable<CertificationResponseDto> Certifications,
    DrivingLicenseResponseDto? DrivingLicense,
    NationalIdentityResponseDto? NationalIdentity);