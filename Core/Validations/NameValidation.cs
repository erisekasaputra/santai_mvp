using System.Text.RegularExpressions;

namespace Core.Validations;

public static partial class NameValidation
{
    [GeneratedRegex(@"^[A-Za-z]+(?: [A-Za-z]+)*$")]
    private static partial Regex NameRegex();

    public static bool IsValidName(string name)
    {
        Regex regex = NameRegex();
        return regex.IsMatch(name);
    } 
}
