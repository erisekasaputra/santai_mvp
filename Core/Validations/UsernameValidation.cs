using System.Text.RegularExpressions;

namespace Core.Validations;

public static partial class UsernameValidation
{
    [GeneratedRegex("^[a-z0-9]+$")]
    private static partial Regex UsernameRegex();

    public static bool IsValidUsername(string username)
    {
        Regex regex = UsernameRegex();
        return regex.IsMatch(username);
    } 
}
