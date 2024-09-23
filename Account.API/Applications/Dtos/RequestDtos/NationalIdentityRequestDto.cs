using Account.API.Extensions;
using Core.Extensions;

namespace Account.API.Applications.Dtos.RequestDtos;

public class NationalIdentityRequestDto(
    string identityNumber,
    string frontSideImageUrl,
    string backSideImageUrl)
{
    public required string IdentityNumber { get; set; } = identityNumber.Clean();
    public required string FrontSideImageUrl { get; set; } = frontSideImageUrl.Clean();
    public required string BackSideImageUrl { get; set; } = backSideImageUrl.Clean();
}
