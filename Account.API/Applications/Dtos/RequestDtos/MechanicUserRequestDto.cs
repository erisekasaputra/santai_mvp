using Account.API.Extensions;

namespace Account.API.Applications.Dtos.RequestDtos;

public class MechanicUserRequestDto(
    Guid identityId,
    string username,
    string email,
    string phoneNumber,
    string timeZoneId,
    AddressRequestDto address,
    IEnumerable<CertificationRequestDto> certifications,
    DrivingLicenseRequestDto drivingLicense,
    NationalIdentityRequestDto nationalIdentity,
    string deviceId)
{
    public Guid IdentityId { get; } = identityId;
    public string Username { get; } = username.Clean();
    public string Email { get; set; } = email.CleanAndLowering();
    public string PhoneNumber { get; set; } = phoneNumber.Clean();
    public string TimeZoneId { get; } = timeZoneId.Clean();
    public AddressRequestDto Address { get; } = address;
    public IEnumerable<CertificationRequestDto> Certifications { get; } = certifications;
    public DrivingLicenseRequestDto DrivingLicense { get; } = drivingLicense;
    public NationalIdentityRequestDto NationalIdentity { get; } = nationalIdentity;
    public string DeviceId { get; } = deviceId;
}
