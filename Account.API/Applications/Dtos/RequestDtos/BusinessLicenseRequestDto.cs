using Account.API.Extensions;

namespace Account.API.Applications.Dtos.RequestDtos;

public class BusinessLicenseRequestDto(
    string LicenseNumber,
    string Name,
    string Description)
{
    public string LicenseNumber { get; } = LicenseNumber.Clean();
    public string Name { get; } = Name.Clean();
    public string Description { get; } = Description.Clean();
}