using System.Text.RegularExpressions;

namespace CustomValidation;

public partial class PasswordValidator
{
    [GeneratedRegex(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).{6,}$")]
    private static partial Regex RegexPasswordValidator();
    public static bool IsValidPassword(string password)
    { 
        return RegexPasswordValidator().IsMatch(password);
    } 
}
