using Account.API.Extensions;

namespace Account.API.Applications.Dtos.RequestDtos;

public class NationalIdentityRequestDto(
    string IdentityNumber,
    string FrontSideImageUrl,
    string BackSideImageUrl)
{
    public string IdentityNumber { get; } = IdentityNumber.Clean();
    public string FrontSideImageUrl { get; } = FrontSideImageUrl.Clean();
    public string BackSideImageUrl { get; } = BackSideImageUrl.Clean();
}
