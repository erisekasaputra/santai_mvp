using Account.API.Extensions;
using Core.Extensions;

namespace Account.API.Applications.Dtos.RequestDtos;

public class UpdateBusinessUserRequestDto(
    string businessName,
    string businessImageUrl,
    string contactPerson,
    string? taxId,
    string? websiteUrl,
    string? description,
    AddressRequestDto address,
    string timeZoneId)
{ 
    public required string BusinessName { get; set; } = businessName.Clean();
    public required string BusinessImageUrl { get; set; } = businessImageUrl.Clean();
    public required string ContactPerson { get; set; } = contactPerson.Clean();
    public string? TaxId { get; } = taxId?.Clean();
    public string? WebsiteUrl { get; } = websiteUrl?.Clean();
    public string? Description { get; } = description?.Clean();
    public required AddressRequestDto Address { get; set; } = address;
    public string TimeZoneId { get; } = timeZoneId.Clean();
}
