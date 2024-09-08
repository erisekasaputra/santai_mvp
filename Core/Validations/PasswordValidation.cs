using System.Text.RegularExpressions;

namespace Core.Validations;

public partial class PasswordValidation
{
    [GeneratedRegex(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).{6,}$")]
    private static partial Regex RegexPasswordValidation();
    public static bool IsValidPassword(string password)
    {
        return RegexPasswordValidation().IsMatch(password);
    }
}
