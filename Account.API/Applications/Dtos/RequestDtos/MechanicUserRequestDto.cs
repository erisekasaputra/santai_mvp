using Account.API.Extensions;

namespace Account.API.Applications.Dtos.RequestDtos;

public class MechanicUserRequestDto(
    Guid IdentityId,
    string Username,
    string Email,
    string PhoneNumber,
    string TimeZoneId,
    AddressRequestDto Address,
    IEnumerable<CertificationRequestDto> Certifications,
    DrivingLicenseRequestDto DrivingLicenseRequestDto,
    NationalIdentityRequestDto NationalIdentityRequestDto,
    string DeviceId)
{
    public Guid IdentityId { get; } = IdentityId;
    public string Username { get; } = Username.Clean();
    public string Email { get; } = Email.CleanAndLowering();
    public string PhoneNumber { get; } = PhoneNumber.Clean();
    public string TimeZoneId { get; } = TimeZoneId.Clean();
    public AddressRequestDto Address { get; } = Address;
    public IEnumerable<CertificationRequestDto> Certifications { get; } = Certifications;
    public DrivingLicenseRequestDto DrivingLicenseRequestDto { get; } = DrivingLicenseRequestDto;
    public NationalIdentityRequestDto NationalIdentityRequestDto { get; } = NationalIdentityRequestDto;
    public string DeviceId { get; } = DeviceId;
}
