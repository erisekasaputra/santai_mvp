namespace Identity.API.Applications.Dto;
public class PasswordForgotRequest
{
    public required string PhoneNumber { get; set; } = string.Empty;
}

