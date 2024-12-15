using Account.Domain.Enumerations;

namespace Account.API.Applications.Dtos.ResponseDtos;

public record DrivingLicenseResponseDto(
    Guid Id, 
    string LicenseNumber,
    string FrontSideImageUrl,
    string BackSideImageUrl,
    VerificationState VerificationStatus);
