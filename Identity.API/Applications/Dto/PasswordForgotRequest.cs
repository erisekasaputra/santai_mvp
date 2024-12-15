namespace Identity.API.Applications.Dto;
public class PasswordForgotRequest
{
    public string PhoneNumber { get; set; }

    public PasswordForgotRequest(string phoneNumber)
    {
        PhoneNumber = phoneNumber.Trim();
    }
}

