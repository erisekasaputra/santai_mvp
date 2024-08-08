using Account.API.Extensions;

namespace Account.API.Applications.Dtos.RequestDtos;

public class DrivingLicenseRequestDto(
    string LicenseNumber,
    string FrontSideImageUrl,
    string BackSideImageUrl)
{
    public string LicenseNumber { get; } = LicenseNumber.Clean();
    public string FrontSideImageUrl { get; } = FrontSideImageUrl.Clean();
    public string BackSideImageUrl { get; } = BackSideImageUrl.Clean();
}
