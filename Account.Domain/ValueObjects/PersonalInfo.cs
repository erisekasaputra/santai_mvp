using Account.Domain.Enumerations;
using Account.Domain.Exceptions;
using Account.Domain.SeedWork;

namespace Account.Domain.ValueObjects;

public class PersonalInfo : ValueObject
{
    public string FirstName { get; set; }
    public string? MiddleName { get; set; }
    public string? LastName { get; set; }
    public DateTime DateOfBirthUtc { get; set; }
    public Gender Gender { get; set; }
    public string? ProfilePictureUrl { get; set; } 

    public PersonalInfo()
    {
        FirstName = null!;
    }
    
    public PersonalInfo(string firstName, string? middleName, string? lastName, DateTime dateOfBirthUtc, Gender gender, string? profilePictureUrl)
    {
        ArgumentNullException.ThrowIfNull(firstName, nameof(firstName));

        if (dateOfBirthUtc >= DateTime.UtcNow) 
        {
            throw new DomainException("Can not set bithday in the past");
        }

        FirstName = firstName;
        MiddleName = middleName;
        LastName = lastName;
        DateOfBirthUtc = dateOfBirthUtc;
        Gender = gender;
        ProfilePictureUrl = profilePictureUrl;
    }  
    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return FirstName;
        yield return MiddleName;
        yield return LastName;
        yield return DateOfBirthUtc;
        yield return Gender;
    }
}