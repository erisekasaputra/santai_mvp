using Account.Domain.Enumerations;
using Account.Domain.SeedWork;

namespace Account.Domain.ValueObjects;

public class PersonalInfo : ValueObject
{
    public string FirstName { get; set; }
    public string? MiddleName { get; set; }
    public string? LastName { get; set; }
    public DateTime DateOfBirth { get; set; }
    public Gender Gender { get; set; }
    public string ProfilePictureUrl { get; set; }

    public PersonalInfo(string firstName, string? middleName, string? lastName, DateTime dateOfBirth, Gender gender, string profilePictureUrl)
    {
        FirstName = firstName;
        MiddleName = middleName;
        LastName = lastName;
        DateOfBirth = dateOfBirth;
        Gender = gender;
        ProfilePictureUrl = profilePictureUrl;
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return FirstName;
        yield return MiddleName;
        yield return LastName;
        yield return DateOfBirth;
        yield return Gender;
    }
}