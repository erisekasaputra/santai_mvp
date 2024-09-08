using Account.API.Extensions;
using Core.Extensions;

namespace Account.API.Applications.Dtos.RequestDtos;

public class BusinessLicenseRequestDto(
    string licenseNumber,
    string name,
    string description)
{
    public string LicenseNumber { get; set; } = licenseNumber.Clean();
    public string Name { get; } = name.Clean();
    public string Description { get; } = description.Clean();
}