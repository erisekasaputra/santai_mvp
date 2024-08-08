using Account.API.Extensions;
using Account.Domain.Enumerations;

namespace Account.API.Applications.Dtos.RequestDtos;

public class PersonalInfoRequestDto(
    string FirstName,
    string? MiddleName,
    string? LastName,
    DateTime DateOfBirth,
    Gender Gender,
    string? ProfilePicture)
{
    public string FirstName { get; } = FirstName.Clean();
    public string? MiddleName { get; } = MiddleName?.Clean();
    public string? LastName { get; } = LastName?.Clean();
    public DateTime DateOfBirth { get; } = DateOfBirth;
    public Gender Gender { get; } = Gender;
    public string? ProfilePicture { get; } = ProfilePicture?.Clean();
}
