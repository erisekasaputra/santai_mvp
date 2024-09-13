namespace Ordering.API.Applications.Dtos.Responses;


public class AccountIdentityPersonalInfoResponseDto
{
    public string FirstName { get; set; }
    public string? MiddleName { get; set; }
    public string? LastName { get; set; }
    public DateTime DateOfBirth { get; set; }
    public string? ProfilePicture { get; set; }
    public AccountIdentityPersonalInfoResponseDto(
        string firstName,
        string? middleName,
        string? lastName,
        DateTime dateOfBirth,
        string? profilePicture)
    {
        FirstName = firstName;
        MiddleName = middleName;
        LastName = lastName;
        DateOfBirth = dateOfBirth;
        ProfilePicture = profilePicture;
    }

    public string ToFullName
    {
        get
        {
            return $"{FirstName} {MiddleName} {LastName}";
        }
    }
}
