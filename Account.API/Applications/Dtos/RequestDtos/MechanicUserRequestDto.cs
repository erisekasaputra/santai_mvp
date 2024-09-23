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
    NationalIdentityRequestDto nationalIdentity )
{  
    public required string TimeZoneId { get; set; } = timeZoneId.Clean();
    public string? ReferralCode { get; set; } = referralCode?.Clean();
    public required PersonalInfoRequestDto PersonalInfo { get; set; } = personalInfo;
    public required AddressRequestDto Address { get; set; } = address;
    public required IEnumerable<CertificationRequestDto> Certifications { get; set; } = certifications;
    public required DrivingLicenseRequestDto DrivingLicense { get; set; } = drivingLicense;
    public required NationalIdentityRequestDto NationalIdentity { get; set; } = nationalIdentity; 
}
