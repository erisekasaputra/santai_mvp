namespace Account.API.Applications.Dtos.ResponseDtos;

public record DrivingLicenseResponseDto(
    string LicenseNumber,
    string FrontSideImageUrl,
    string BackSideImageUrl);
