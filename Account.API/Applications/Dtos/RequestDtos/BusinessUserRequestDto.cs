using Account.API.Extensions;

namespace Account.API.Applications.Dtos.RequestDtos;

public class BusinessUserRequestDto(
    Guid IdentityId,
    string Username,
    string Email,
    string PhoneNumber,
    string TimeZoneId,
    AddressRequestDto Address,
    string BusinessName,
    string ContactPerson,
    string? TaxId,
    string? WebsiteUrl,
    string? BusinessDescription,
    string? ReferralCode,
    IEnumerable<BusinessLicenseRequestDto> BusinessLicenses,
    IEnumerable<StaffRequestDto> Staffs)
{
    public Guid IdentityId { get; } = IdentityId;
    public string Username { get; } = Username.Clean();
    public string Email { get; } = Email.CleanAndLowering();
    public string PhoneNumber { get; } = PhoneNumber.Clean();
    public string TimeZoneId { get; } = TimeZoneId.Clean();
    public AddressRequestDto Address { get; } = Address;
    public string BusinessName { get; } = BusinessName.Clean();
    public string ContactPerson { get; } = ContactPerson.Clean();
    public string? TaxId { get; } = TaxId?.Clean();
    public string? WebsiteUrl { get; } = WebsiteUrl?.Clean();
    public string? BusinessDescription { get; } = BusinessDescription?.Clean();
    public string? ReferralCode { get; } = ReferralCode?.Clean();
    public IEnumerable<BusinessLicenseRequestDto> BusinessLicenses { get; } = BusinessLicenses;
    public IEnumerable<StaffRequestDto> Staffs { get; } = Staffs;
}
