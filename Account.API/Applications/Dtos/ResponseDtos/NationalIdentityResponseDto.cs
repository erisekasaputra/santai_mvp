namespace Account.API.Applications.Dtos.ResponseDtos;

public record NationalIdentityResponseDto(
    string IdentityNumber,
    string FrontSideImageUrl,
    string BackSideImageUrl);