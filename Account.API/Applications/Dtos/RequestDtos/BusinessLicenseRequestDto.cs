using Account.API.Extensions;
using Core.Extensions;

namespace Account.API.Applications.Dtos.RequestDtos;

public class BusinessLicenseRequestDto(
    string licenseNumber,
    string name,
    string description)
{
    public required string LicenseNumber { get; set; } = licenseNumber.Clean();
    public required string Name { get; set; } = name.Clean();
    public required string Description { get; set; } = description.Clean();
}