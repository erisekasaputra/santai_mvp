using System.Net.Mail;

namespace Core.Validations;

public class EmailValidation
{
    public static bool IsValidEmail(string email)
    {
        try
        {
            var address = new MailAddress(email);
            return address.Address == email;
        }
        catch
        {
            return false;
        }
    } 
}
