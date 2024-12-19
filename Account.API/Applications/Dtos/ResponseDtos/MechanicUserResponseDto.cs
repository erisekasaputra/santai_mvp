using Account.Domain.Enumerations;

namespace Account.API.Applications.Dtos.ResponseDtos;

public record MechanicUserResponseDto(
    Guid Id, 
    string? Email,
    string? PhoneNumber,
    string TimeZoneId,
    PersonalInfoResponseDto PersonalInfo,
    LoyaltyProgramResponseDto Loyalty, 
    ReferralProgramResponseDto? Referral,
    AddressResponseDto Address,
    IEnumerable<CertificationResponseDto>? Certifications,
    DrivingLicenseResponseDto? DrivingLicense,
    NationalIdentityResponseDto? NationalIdentity,
    decimal Rating,
    DateTime CreatedAt,
    int TotalEntireJob,
    int TotalCancelledJob,
    int TotalEntireJobBothCompleteIncomplete,
    int TotalCompletedJob,
    VerificationState IsVerified,
    bool IsActive);


 