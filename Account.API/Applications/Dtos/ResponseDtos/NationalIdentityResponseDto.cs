using Account.Domain.Enumerations;

namespace Account.API.Applications.Dtos.ResponseDtos;

public record NationalIdentityResponseDto(
    Guid Id,
    string IdentityNumber,
    string FrontSideImageUrl,
    string BackSideImageUrl,
    VerificationState VerificationStatus);