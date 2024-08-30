using Account.API.Extensions;

namespace Account.API.Applications.Dtos.RequestDtos;

public class BusinessUserRequestDto( 
    string? email,
    string phoneNumber, 
    string timeZoneId,
    AddressRequestDto address,
    string businessName,
    string contactPerson,
    string? taxId,
    string? websiteUrl,
    string? businessDescription,
    string? referralCode,
    IEnumerable<BusinessLicenseRequestDto> businessLicenses,
    IEnumerable<StaffRequestDto> staffs)
{ 
    public string? Email { get; set; } = email?.CleanAndLowering(); 
    public string PhoneNumber { get; set; } = phoneNumber.Clean();
    public string TimeZoneId { get; } = timeZoneId.Clean();
    public AddressRequestDto Address { get; } = address;
    public string BusinessName { get; } = businessName.Clean();
    public string ContactPerson { get; } = contactPerson.Clean();
    public string? TaxId { get; set; } = taxId?.Clean();
    public string? WebsiteUrl { get; } = websiteUrl?.Clean();
    public string? BusinessDescription { get; } = businessDescription?.Clean();
    public string? ReferralCode { get; } = referralCode?.Clean();
    public IEnumerable<BusinessLicenseRequestDto> BusinessLicenses { get; } = businessLicenses;
    public IEnumerable<StaffRequestDto> Staffs { get; } = staffs;
}
