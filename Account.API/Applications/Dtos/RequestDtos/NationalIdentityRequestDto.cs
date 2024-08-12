using Account.API.Extensions;

namespace Account.API.Applications.Dtos.RequestDtos;

public class NationalIdentityRequestDto(
    string identityNumber,
    string frontSideImageUrl,
    string backSideImageUrl)
{
    public string IdentityNumber { get; set; } = identityNumber.Clean();
    public string FrontSideImageUrl { get; } = frontSideImageUrl.Clean();
    public string BackSideImageUrl { get; } = backSideImageUrl.Clean();
}
