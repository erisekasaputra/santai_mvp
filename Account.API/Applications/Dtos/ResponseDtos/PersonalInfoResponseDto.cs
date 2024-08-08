using Account.Domain.Enumerations;

namespace Account.API.Applications.Dtos.ResponseDtos;

public record PersonalInfoResponseDto(
    string FirstName,
    string? MiddleName,
    string? LastName,
    DateTime DateOfBirth,
    Gender Gender,
    string? ProfilePicture);