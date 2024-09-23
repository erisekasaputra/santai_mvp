using Account.API.Extensions;
using Core.Extensions;

namespace Account.API.Applications.Dtos.RequestDtos;

public class DrivingLicenseRequestDto(
    string licenseNumber,
    string frontSideImageUrl,
    string backSideImageUrl)
{
    public required string LicenseNumber { get; set; } = licenseNumber.Clean();
    public required string FrontSideImageUrl { get; set; } = frontSideImageUrl.Clean();
    public required string BackSideImageUrl { get; set; } = backSideImageUrl.Clean();
}
