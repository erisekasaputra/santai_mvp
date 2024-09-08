using Account.API.Extensions;
using Core.Extensions;

namespace Account.API.Applications.Dtos.RequestDtos;

public class MechanicUserRequestDto(
    string timeZoneId,
    string? referralCode,
    PersonalInfoRequestDto personalInfo,
    AddressRequestDto address,
    IEnumerable<CertificationRequestDto> certifications,
    DrivingLicenseRequestDto drivingLicense,
    NationalIdentityRequestDto nationalIdentity,
    string deviceId)
{  
    public string TimeZoneId { get; } = timeZoneId.Clean();
    public string? ReferralCode { get; } = referralCode?.Clean();
    public PersonalInfoRequestDto PersonalInfo { get; set; } = personalInfo;
    public AddressRequestDto Address { get; } = address;
    public IEnumerable<CertificationRequestDto> Certifications { get; } = certifications;
    public DrivingLicenseRequestDto DrivingLicense { get; } = drivingLicense;
    public NationalIdentityRequestDto NationalIdentity { get; } = nationalIdentity;
    public string DeviceId { get; } = deviceId;
}
