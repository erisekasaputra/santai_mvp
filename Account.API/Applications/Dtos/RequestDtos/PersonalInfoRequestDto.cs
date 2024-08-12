using Account.API.Extensions;
using Account.Domain.Enumerations;

namespace Account.API.Applications.Dtos.RequestDtos;

public class PersonalInfoRequestDto(
    string firstName,
    string? middleName,
    string? lastName,
    DateTime dateOfBirth,
    Gender gender,
    string? profilePictureUrl)
{
    public string FirstName { get; } = firstName.Clean();
    public string? MiddleName { get; } = middleName?.Clean();
    public string? LastName { get; } = lastName?.Clean();
    public DateTime DateOfBirth { get; } = dateOfBirth;
    public Gender Gender { get; } = gender;
    public string? ProfilePictureUrl { get; } = profilePictureUrl?.Clean();
}
