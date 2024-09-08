using Account.API.Extensions;
using Core.Extensions;

namespace Account.API.Applications.Dtos.RequestDtos;

public class DrivingLicenseRequestDto(
    string licenseNumber,
    string frontSideImageUrl,
    string backSideImageUrl)
{
    public string LicenseNumber { get; set; } = licenseNumber.Clean();
    public string FrontSideImageUrl { get; } = frontSideImageUrl.Clean();
    public string BackSideImageUrl { get; } = backSideImageUrl.Clean();
}
