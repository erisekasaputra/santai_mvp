using Account.Domain.Enumerations;
using Account.Domain.ValueObjects;

namespace Account.API.DTOs;

public class BusinessUserDto
{
    public string BusinessName { get; set; }

    public string TaxId { get; set; }

    public string ContactPerson { get; set; }

    public string? WebsiteUrl { get; set; }

    public string? Description { get; set; }

    public ICollection<BusinessLicenseDto> BusinessLicenses { get; set; } 

    public BusinessUserDto(
        string username,
        string email,
        string phoneNumber,
        AccountStatus accountStatus,
        ICollection<AddressDto> address,
        string businessName,
        string taxId,
        string contactPerson,
        ICollection<BusinessLicenseDto> businessLicenses,
        string websiteUrl,
        string description) 
    {
        
    }

}
