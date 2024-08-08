using System.Text.RegularExpressions;

namespace Account.API.Extensions;

public static partial class NameExtension
{
    [GeneratedRegex(@"^[A-Za-z]+(?: [A-Za-z]+)*$")]
    private static partial Regex NameRegex();

    public static bool IsValidName(string name)
    {
        Regex regex = NameRegex();
        return regex.IsMatch(name);
    } 
}
