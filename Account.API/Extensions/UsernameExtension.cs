using System.Text.RegularExpressions;

namespace Account.API.Extensions;

public static partial class UsernameExtension
{
    [GeneratedRegex("^[a-z0-9]+$")]
    private static partial Regex UsernameRegex();

    public static bool IsValidUsername(string username)
    {
        Regex regex = UsernameRegex();
        return regex.IsMatch(username);
    } 
}
