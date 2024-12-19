using Core.Extensions;

namespace Account.API.Applications.Dtos.RequestDtos;

public class BusinessUserRequestDto(  
    string phoneNumber, 
    string timeZoneId,
    AddressRequestDto address,
    string businessName,
    string businessImageUrl,
    string contactPerson,
    string? taxId,
    string? websiteUrl,
    string? businessDescription,
    string? referralCode,
    string password,
    IEnumerable<BusinessLicenseRequestDto> businessLicenses,
    IEnumerable<StaffRequestDto> staffs)
{  
    public required string PhoneNumber { get; set; } = phoneNumber.Clean();
    public required string TimeZoneId { get; set; } = timeZoneId.Clean();
    public required AddressRequestDto Address { get; set; } = address;
    public required string BusinessName { get; set; } = businessName.Clean();
    public required string BusinessImageUrl { get; set; } = businessImageUrl.Clean();
    public required string ContactPerson { get; set; } = contactPerson.Clean();
    public string? TaxId { get; set; } = taxId?.Clean();
    public string? WebsiteUrl { get; set; } = websiteUrl?.Clean();
    public string? BusinessDescription { get; set; } = businessDescription?.Clean();
    public string? ReferralCode { get; set; } = referralCode?.Clean();
    public required string Password { get; set; } = password.Clean();
    public required IEnumerable<BusinessLicenseRequestDto> BusinessLicenses { get; set; } = businessLicenses;
    public required IEnumerable<StaffRequestDto> Staffs { get; set; } = staffs;
}
