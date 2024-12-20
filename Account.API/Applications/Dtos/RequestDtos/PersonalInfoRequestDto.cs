 
using Account.Domain.Enumerations;
using Core.Extensions;

namespace Account.API.Applications.Dtos.RequestDtos;

public class PersonalInfoRequestDto(
    string firstName,
    string? middleName,
    string? lastName,
    DateTime? dateOfBirth,
    Gender gender,
    string? profilePictureUrl)
{
    public required string FirstName { get; set; } = firstName.Clean();
    public string? MiddleName { get; set; } = middleName?.Clean();
    public string? LastName { get; set; } = lastName?.Clean();
    public DateTime? DateOfBirth { get; set; } = dateOfBirth;
    public required Gender Gender { get; set; } = gender;
    public string? ProfilePictureUrl { get; set; } = profilePictureUrl?.Clean();
}
